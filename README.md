Note: This was coded in C#
# PhysKit v0.1.0
- Grabbing and manipulating objects in a physics based way
- Independently grab objects with your left and right hands
- A physics based character controller
- Made in Godot .NET Build 4.7

# To Use
- Copy the contents of InputMappings.txt, and paste them in your project.godot file under the [Input] heading. This adds all the needed input mappings to your project
- Add Player.tscn to your desired scene. World.tscn is provided for you if you want a ready-to-go demo scene
- Remove any other cameras in your scene, Player.tscn already has one
- PlayerController.cs is found on the root Rigidbody3D node on the player. You can customize the player's movement and jump behavior here.
- PlayerInteraction.cs is found on the Interact Raycast node, which is a child of the Camera3D node on the player. You can customize the player's grab behavior here.
- To grab an object with your left hand, press Q. To grab with your right hand, press E.
  - Yes, it makes more sense to use left/right mouse buttons. However, I will soon be adding the ability to "Use" the objects you are holding in each hand (such as firing a weapon). I thought it made more sense for left/right mouse buttons to be used for using objects.

### Grab Point (Node3d)
- If you want the player to grab an object in the same spot every time, add a Node3D with the name "Grab Point", and position it where you want the player to grab the object
- For example, if your game has a sword, you would want the player to always grab the sword by the handle. So you would add a "Grab Point" child node to the sword and position that "Grab Point" node on the handle of the sword.

### Metadata Properties
#### Grabbable (boolean)
- Add to any rigidbody you want to pick up. Must be set to true
#### Rotation Override
- Add to any grabbable object you want to have a preset rotation relative to the player
- For example, if the player picked up a sword, you would want the sword to "snap" to the same rotation relative to the camera no matter where the player is facing when they grab it

# Future Plans
- Add the ability to use the items you are holding in your hands, such as shooting a gun
- This tool is being developed for a PvE game, so this tool will slowly become more and more optimized for physics based combat. However, I will make sure that this tool can still be used just as effectively in any kind of game.


Since, this toolkit is being developed for my own game, so you can expect continued updates.
If you find any bugs, or have any suggestions, open an [Issue](https://github.com/Rockchuck27/Godot-PhysKit/issues)
