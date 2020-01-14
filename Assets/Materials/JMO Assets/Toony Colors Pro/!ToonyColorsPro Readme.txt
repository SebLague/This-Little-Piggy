Toony Colors Pro, version 1.7
formerly Toony Gooch Pro
2014/05/19
© 2013,2014 Jean Moreno
=============================

USAGE
-----
When imported, the package only extracts the Toony Colors Pro shaders with One Directional Light
support (+ other lights as vertex lit) to avoid a too long importing time (there are 80 shader
files total, so that can take a while to import/compile).
The other shaders are located in .zip files:
- ToonyColors_Shaders_MultipleLights:			contains the shaders supporting multiple lights
- ToonyColors_Shaders_Outline_Z-Correction:		contains the Z Correction shaders (see below)
- ToonyColors_Shaders_OutlineConstant_Z-Correction	contains the Z Correction with Constant Size outline (see below)
/!\ Shaders must be extracted in the same folder as "TGP_Include.cginc"!


Z-CORRECTION
------------
Sometimes when using outlines on complex geometry, some graphical artefacts appear (for example
around the mouth of a character); the Z-Correction shaders are here to try to remove or smooth
these artefacts.
/!\ Only use them if you see noticeable artefacts as they are a little more costly on the GPU.
/!\ Shaders must be extracted in the same folder as "TGP_Include.cginc"!


SHADERS SPEED COMPARISON
------------------------
From cheapest to more expensive (on mobile):
* Basic
* Bumped
* Basic Rim
* Specular
* Specular Rim
* Bumped Rim
* Bumped Specular
* Bumped Specular Rim
Tests have been made on an Android 4.0.4 device with a PowerVR-based GPU, results might differ on other GPUs.


MOBILE RECOMMENDATIONS
----------------------
* Only use expensive shaders for models that are seen frequently and close to the camera: 
for example, use a Bumped Specular shader only on the main character in a 3rd person view game.
* Specular and Rim lighting greatly reduce performances, so only use them when it's really necessary!


TIPS
----
* You can achieve a reverse Rim Light by setting the Rim Power to the far left of the slider, and 
adjusting the Rim Color with a very low alpha value!
* Use the Z-Correct shaders when you see Outlines artefacts from complex geometry (for example
character mouths); adjust the outlines with the "Z Correction" slider
* Outline shaders aren't very expensive on the GPU, but they double the number of vertices/triangles processed!


PLEASE LEAVE A REVIEW OR RATE THE PACKAGE IF YOU FIND IT USEFUL!
Enjoy! :)


CONTACT
-------
Questions, suggestions, help needed?
Contact me at:
jean.moreno.public+unity@gmail.com

I'd be happy to see any shader used in your project, so feel free to drop me a line about that! :)


UPDATE NOTES
------------
v1.7
- added alpha output to shader files (RenderTextures should now work for real)
- Constant Outline shaders now take the object's uniform scale into account

v1.6
- fixed alpha output to 0 in lighting model, would cause problems with Render Textures previously
- fixed Warnings in Unity 4+ versions
- fixed shader Warnings for D3D11 and D3D11_9X compilers
- re-enabled ZWrite by default for outlines, would cause them to not show over skyboxes previously

v1.5
- fixed the specular lighting algorithm, would cause glitches with small light ranges

v1.4

- changed name to "Toony Colors"
- fixed Bump Maps Substance compatibility (WARNING: you may have to re-set the Normal Maps in your materials)

v1.3
- added Rim Outline shaders

v1.2
- added JMO Assets menu (Window -> JMO Assets), to check for updates or get support

v1.1
- Rim lighting is much faster! (excepted on Rim+Bumped shaders)

v1.01
- Included Demo Scene

v1.0
- Initial Release