# Simple Free Camera
Maze procedural generator

## description :

generate a maze, with paths and positions, all linked by a directed nodal system.

QUICK TOUR :

-use generate maze to generate the maze.
-is experiencing any issue with generation, use "kill" to stop any generation process
-choose update type to choose how fast generation will proceed, by default, generation is instant, but you can choose to generate only one update per x sec, or one update per frame, this is most commonly use for debugging
-the maze contain directed paths, and is made so that the first position of the maze is the "root" this as one partricular property : you can change de root position while keeping the maze a "perfect" one, use "move root" to randomly move the root between 4 neighbor positions
-number of root iterations set how many times root move randomly when you hit the move root button once.

-Maze parameters :
width : the width of maze in number of tiles
height : height of maze in number of tiles
entries pos : let you set the entries of maze, in coordinates, and maze coordinates start at 0 at the botom left, that means, to set an entry for example, at the left of the position 0, 0, you need to set the entry pos at -1, 0. WARNING : it is possible to set an entry inside the maze, however, no features were implemented, this is quite experimental and can lead to some bugs.
faced axis : how the maze is rendered in world coordinates.

-Maze Data parameters : open an editor window to let you freely edit your maze : 
left click to select a position or a path, or to switch a path state(between walkable path or wall)
right click to unselect everything
when part of maze is selected : 
make origin of paths : make all 4 neighbor paths directed away from position
make target of paths : make all 4 neighbor paths directed towards this position
swap side : inverse direction of this path
note : for now, the direction of path that is a wall as no effect on anything

-Maze build parameters :
Build maze : build maze with the given prefabs
Clear maze build : remove every objects of maze and clean it
Procedural Generation : WIP : if enabled, use procedural mesh generation instead of prefabs
wall prefab : object used to build a wall
wall border prefab : object used to build a wall at the border of the maze


WARNING : with big maze, enabling some drawers in debug options can be really(REALLY) ressource consumming


## Links :
["Youtube Channel"](https://www.youtube.com/channel/UC-_DDdI316_BYs7HlO260OA)

["Home Page"](https://github.com/Light974-M/UnityPersonalDataBank)