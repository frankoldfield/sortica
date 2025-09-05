---
layout: single
title: "Welcome to Sortica's WebPage!"
---

<h1>Overview</h1>
<p>Sortica scene drafts!</p>

<!-- Carga A-Frame -->
<script src="https://aframe.io/releases/1.2.0/aframe.min.js"></script>

<!-- Contenedor principal -->
<div style="display: flex; flex-wrap: wrap; gap: 20px; justify-content: center;">

  <!-- Escena Sortica HQ -->
  <div style="flex: 1 1 45%; max-width: 600px; display: flex; flex-direction: column; align-items: center;">
    <div style="position: relative; width: 100%; padding-top: 56.25%;"> <!-- 16:9 ratio -->
      <a-scene embedded style="position: absolute; top: 0; left: 0; width: 100%; height: 100%;">
        <a-assets>
          <img id="VR_Sortica" src="{{ '/assets/scenes/sortica_hq.png' | relative_url }}">
        </a-assets>
        <a-sky src="#VR_Sortica"></a-sky>
        <a-camera position="0 0 0" fov="90"></a-camera>
      </a-scene>
    </div>
    <p>Sortica HQ Scene</p>
  </div>

  <!-- Escena Tutorial -->
  <div style="flex: 1 1 45%; max-width: 600px; display: flex; flex-direction: column; align-items: center;">
    <div style="position: relative; width: 100%; padding-top: 56.25%;"> <!-- 16:9 ratio -->
      <a-scene embedded style="position: absolute; top: 0; left: 0; width: 100%; height: 100%;">
        <a-assets>
          <img id="VR_Tutorial" src="{{ '/assets/scenes/tutorial.png' | relative_url }}">
        </a-assets>
        <a-sky src="#VR_Tutorial"></a-sky>
        <a-camera position="0 0 0" fov="90"></a-camera>
      </a-scene>
    </div>
    <p>Tutorial Scene</p>
  </div>

</div>

<!-- Responsive para móviles -->
<style>
  @media (max-width: 800px) {
    div[style*="flex: 1 1 45%"] {
      flex: 1 1 100% !important;
    }
  }
</style>
