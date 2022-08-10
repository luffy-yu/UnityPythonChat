# refer: https://gitlab.freedesktop.org/gstreamer/gst-examples/-/blob/1.18/webrtc/signalling/session-client.py

import argparse
import asyncio
import json
import ssl
import sys
import uuid

import websockets

parser = argparse.ArgumentParser(formatter_class=argparse.ArgumentDefaultsHelpFormatter)
parser.add_argument('--url', default='wss://localhost:8943', help='URL to connect to')
parser.add_argument('--call', default=None, help='uid of peer to call')

options = parser.parse_args(sys.argv[1:])

SERVER_ADDR = options.url
CALLEE_ID = options.call
# PEER_ID = 'ws-test-client-' + str(uuid.uuid4())[:6]
PEER_ID = 'PC-' + '666666'


def reply_sdp_ice(msg):
    # Here we'd parse the incoming JSON message for ICE and SDP candidates
    print("Got: " + msg)
    reply = json.dumps({'sdp': 'reply sdp'})
    print("Sent: " + reply)
    return reply


def send_sdp_ice():
    reply = json.dumps({'sdp': 'initial sdp'})
    print("Sent: " + reply)
    return reply


async def hello():
    async with websockets.connect(SERVER_ADDR, ssl=None) as ws:
        await ws.send('HELLO ' + PEER_ID)
        assert (await ws.recv() == 'HELLO')

        # Initiate call if requested
        if CALLEE_ID:
            await ws.send('SESSION {}'.format(CALLEE_ID))

        # Receive messages
        sent_sdp = False
        while True:
            msg = await ws.recv()
            print(msg)
            if msg.startswith('ERROR'):
                # On error, we bring down the webrtc pipeline, etc
                print('{!r}, exiting'.format(msg))
                return
            if sent_sdp:
                print('Got reply sdp: ' + msg)
                return  # Done
            if CALLEE_ID:
                if msg == 'SESSION_OK':
                    await ws.send(send_sdp_ice())
                    sent_sdp = True
                else:
                    print('Unknown reply: {!r}, exiting'.format(msg))
                    return
            if msg.startswith('DATA'):
                # reply something meaningful
                await ws.send(msg + '\'s reply from PYTHON')
            else:
                await ws.send(reply_sdp_ice(msg))
                # return  # Done


print('Our uid is {!r}'.format(PEER_ID))

try:
    asyncio.get_event_loop().run_until_complete(hello())
except websockets.exceptions.InvalidHandshake:
    print('Invalid handshake: are you sure this is a websockets server?\n')
    raise
except ssl.SSLError:
    print('SSL Error: are you sure the server is using TLS?\n')
    raise
