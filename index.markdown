---
layout: single
title: "Welcome to the Sortica development page"
permalink: /
classes: wide


header:
  overlay_color: "#000"
  overlay_filter: "0.4"
  overlay_image: /assets/images/portada.jpg
  actions:
    - label: "Blog"
      url: "/blog/"
      class: "btn btn--primary"
    - label: "About"
      url: "/about/"
      class: "btn btn--inverse"

intro: 
  - excerpt: "This website's purpose is documenting both the development and the use of a Serious Game called Sortica."

feature_row:
  - image_path: /assets/images/black.png
    alt: "scenes.png"
    title: "Scenes"
    excerpt: "VR Scene drafts"
    url: "/scenes"
    btn_label: "See more"
    btn_class: "btn--primary"

  - image_path: /assets/images/purple.png
    alt: "gdd.png"
    title: "GDD"
    excerpt: "Game Design Document"
    url: "/gdd"
    btn_label: "See more"
    btn_class: "btn--primary"

  - image_path: /assets/images/red.png
    alt: "gameloop.png"
    title: "Gameloop"
    excerpt: "Gameloop flowchart"
    url: "/gameloop"
    btn_label: "See more"
    btn_class: "btn--primary"

  - image_path: /assets/images/soft-blue.png
    alt: "LOs.png"
    title: "LOs"
    excerpt: "Learning Objectives"
    url: "/learningobjectives"
    btn_label: "See more"
    btn_class: "btn--primary" 

  - image_path: /assets/images/blue.png
    alt: "accessibility.png"
    title: "Accessibility"
    excerpt: "Accessibility features and guidelines"
    url: "/accessibility"
    btn_label: "See more"
    btn_class: "btn--primary" 

  - image_path: /assets/images/soft-green.png
    alt: "research.png"
    title: "Research"
    excerpt: "Research study"
    url: "/research"
    btn_label: "See more"
    btn_class: "btn--primary" 

# Últimos posts: puedes incluirlos con un include
---

{% include feature_row id="intro" type="center" %}

<p style="font-size:1.2em;">Experience sorting and organizing items in a virtual workshop while learning the logic behind fundamental data structures!</p>

<!-- Breve descripción -->
<p>
The proposed serious game, titled "Sortica - Algorithmic Inventory Sorter," places the user in a post-apocalyptic world where they will have to sort items correctly with the purpose of rebuilding sorting street.
</p>

{% include feature_row %}

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
