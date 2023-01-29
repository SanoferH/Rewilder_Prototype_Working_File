# How to use

## Creating a Metaball Panel

1. Create a Canvas at the Inspector
2. Right click an Canvas > UI > Metaball Panel

### Metaball Panel Settings

* Resolution: Controls the resolution of your metaballs **(Heavy performance impact if high resolution)**
* Background Color
* Anti Aliasing: Amount of anti-aliasing (Resolution independant)
* Color Blending: Amount of color-blending between metaball components
* Correct Coloring: Alternate coloring of metaball components **(Medium performance impact)**

### Metaball Panel Transparency

1. Set alpha to 0
2. Set saturation to 50
> HSV Settings
> * H: 0
> * S: 0
> * V: 50
> * A: 0

## Creating a Metaball

1. Right click a Metaball Panel > UI > UI Metaball

### Metaball Settings

* Color
* Blending: Set the blending amount with the next metaball components
* Round: Boolean that controls metaball shape **(Square or Circle)**
* Roundness: Vector4 that controls the roundness of the edges