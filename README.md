# Flocks 

It's an implementation of a popular flocking algorithm in Unity. A bunch of fish get together and avoid walls, and I think it looks cool. A demonstration video can be found at https://youtu.be/hPZFbJlPhZc.

## Flocking algorithm
The way this algorithm works, is each fish looks at the enviroment and other fish around itself. After looking around, it follows 4 simple rules:
- **Cohesion** - When a fish sees a big flock, it tries to swim towards its center
- **Alignment** - Each fish tries to swim in roughly the same direction as all other fish
- **Seperation** - When 2 fish are too close together, they try to swim apart from each other
- **Avoidance** - All fish try to avoid walls

These four rules are everything that's needed to decide the movement of each fish. How strong fish are affected by these can be changed inside Unity Editor.

## Screeshots
![image](https://github.com/Krzyzan42/FlocksUnity/assets/100627976/01cac34a-8a5c-4864-8fa9-e918c345849a)
