---
layout: single
title: "Scenes"
permalink: /scenes
---

<!-- Escenas A-Frame -->
<script src="https://aframe.io/releases/1.2.0/aframe.min.js"></script>
<div style="display: flex; flex-wrap: wrap; gap: 20px; justify-content: center; margin-bottom: 40px;">

  <!-- Escena 1 -->
  <div style="flex: 1 1 45%; max-width: 600px; display: flex; flex-direction: column; align-items: center;">
    <div style="position: relative; width: 100%; padding-top: 56.25%;">
      <a-scene embedded style="position: absolute; top: 0; left: 0; width: 100%; height: 100%;">
        <a-assets>
          <img id="VR_Sortica" src="{{ '/assets/scenes/sortica_hq.png' | relative_url }}">
        </a-assets>
        <a-sky src="#VR_Sortica"></a-sky>
        <a-camera position="0 0 0" fov="90"></a-camera>
      </a-scene>
    </div>
    <p style="font-weight:bold;">Sortica HQ Preview</p>
  </div>

  <!-- Escena 2 -->
  <div style="flex: 1 1 45%; max-width: 600px; display: flex; flex-direction: column; align-items: center;">
    <div style="position: relative; width: 100%; padding-top: 56.25%;">
      <a-scene embedded style="position: absolute; top: 0; left: 0; width: 100%; height: 100%;">
        <a-assets>
          <img id="VR_Tutorial" src="{{ '/assets/scenes/tutorial.png' | relative_url }}">
        </a-assets>
        <a-sky src="#VR_Tutorial"></a-sky>
        <a-camera position="0 0 0" fov="90"></a-camera>
      </a-scene>
    </div>
    <p style="font-weight:bold;">Tutorial Level Preview</p>
  </div>

  <div style="flex: 1 1 45%; max-width: 600px; display: flex; flex-direction: column; align-items: center;">
    <iframe width="560" height="315" 
    src="https://www.youtube.com/embed/J1qI38jNBHg" 
    frameborder="0" 
    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" 
    allowfullscreen>
    </iframe>
  </div>

  <div style="flex: 1 1 45%; max-width: 600px; display: flex; flex-direction: column; align-items: center;">
    <iframe width="560" height="315" 
    src="https://www.youtube.com/embed/gBo_HC10xfk" 
    frameborder="0" 
    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" 
    allowfullscreen>
    </iframe>
  </div>

</div>