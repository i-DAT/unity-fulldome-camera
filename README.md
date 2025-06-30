# Unity Fulldome Camera

Unity 6 camera script to render to a fulldome using a fisheye projection.
This uses a cubemap rendering approach which renders the scene 6 times, so performance in complicated scenes will suffer.

<p align="center">
    <img src="https://github.com/user-attachments/assets/38c4dbee-fd76-478f-bf94-df8e1b913195">
</p>

## Installation

Follow the [package install guide](https://docs.unity3d.com/6000.1/Documentation/Manual/upm-ui-giturl.html), using this repository URL.

Alternatively, download the contents [`Runtime/`](Runtime/) and place the scripts into your project `Assets` folder.

## Usage

Add the `FisheyeCam.cs` script to your scene camera, then attach the `Fisheye.shader` to the scripts shader slot.

This should immediately change the game preview window to a fisheye projection.
The resolution of the underlying cubemap can be changed with the resolution property, for most domes this shouldn't need to go above 1024.
Often when changing the resolution in the editor, Unity will report errors in the console - these can be ignored and should not affect a built version of the game.

The `FOV` field adjusts the 'zoom' of the camera - most of the time this should stay at 180 though it can be adjusted for some strange visual effects.

The script will create a phantom `FisheyeCanvas` object while it is enabled - don't delete this as it is the render target for the shader.