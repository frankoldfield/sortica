---
layout: single
title: "Sortica - Algorithmic Inventory Sorter"
---

<p style="font-size:1.2em;">Experience sorting and organizing items in a virtual workshop while learning the logic behind fundamental data structures!</p>

<!-- Breve descripción -->
<p>
The proposed serious game, titled "Algorithmic Inventory Sorter," places the user in a virtual workshop where they are tasked with managing items arriving on a conveyor belt. The core mechanic involves interacting with a virtual machine to process these items according to specific rules that mirror the logic of fundamental data structures.
</p>

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

</div>

<!-- Call to Action -->
<h2>Try it yourself</h2>
<p>
Check out the <a href="https://github.com/frankoldfield/sortica/tree/main/code">Unity prototype</a> or browse the <a href="https://github.com/frankoldfield/sortica/tree/main/doc">documentation</a> to see how the game mechanics work. The game is designed to teach programming concepts in a fun, interactive way.
</p>

<h2>Learning Outcomes</h2>
<ul>
  <li>Understand fundamental data structures through hands-on puzzles.</li>
  <li>Experience Stack (LIFO) and Queue (FIFO) behavior interactively.</li>
  <li>Learn to approach problem solving with algorithmic thinking.</li>
</ul>

<h2>More Information</h2>
<p>
For additional resources, updates, or contact, visit our <a href="https://github.com/frankoldfield/sortica/tree/main/doc">documentation page</a> or <a href="https://github.com/frankoldfield/sortica/tree/main/code">project repository</a>.
</p>

<style>
@media (max-width: 800px) {
  div[style*="flex: 1 1 45%"] {
    flex: 1 1 100% !important;
  }
}
</style>
