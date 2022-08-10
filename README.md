# UnityPythonChat 

A fake WebRTC based Unity-Python bi-directional communication sample project.

## Scenario

- Stream camera pose from HoloLens 2 to PC
- Process the pose data (python-based algorithm)
- Stream the processed data from PC to HoloLens 2

> I tried [MixedReality-WebRTC](https://microsoft.github.io/MixedReality-WebRTC/versions/release/1.0/manual/helloworld-unity-createproject.html) and [Unity WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/install.html) but failed.

> It's fake because there's no peer connection.

## Implementation

- Refer to [gst-examples](https://gitlab.freedesktop.org/gstreamer/gst-examples/-/tree/1.18/webrtc/signalling)

## Python side

- A server acts as the bridge that connects two clients, PC and HoloLens 2. 
- A session will be initialized after one peer is registered and another peer is also registered and calls the former peer.
- **After that, two peers exchange data by sending message that starts with `DATA`.**
  - `client.py` Line 66 - 68
  - `BiDirectionalChat.cs` Line 69, and Line 110
  
## Unity side

- IDs of two peers are fixed, you may change as you wish.

## How to run

- Run python server
- Run python client
- Run Unity 
