# Complete TPS Controller


## description :

tps controller that contain every compoent needed for it, animations, accurate steps and controls physics, rotations, and lot more !

TO APPLY THIS CONTROLLER, USE PREFABS AND INCLUDED INPUT ACTION


Look arround and move Character :

![1 Look and move](https://github.com/Light974-M/UnityPersonalDataBank/assets/72139424/a1afff56-6c55-4515-acfd-67aa07ef1ca5)


Jump and Manage Landing everywhere :

![2 Jump and landing](https://github.com/Light974-M/UnityPersonalDataBank/assets/72139424/f5a5dd35-f0cf-4f34-aec5-30bf4e2b84b0)


Make Smooth Sprints and run :

![3 Sprint](https://github.com/Light974-M/UnityPersonalDataBank/assets/72139424/45303a8c-bff7-4fae-8586-070ab941617d)


Crouch :

![4 Crouch](https://github.com/Light974-M/UnityPersonalDataBank/assets/72139424/814ae676-cdab-47ef-ab2b-597baaf24276)


Walk :

![5 Walk](https://github.com/Light974-M/UnityPersonalDataBank/assets/72139424/3e8b87b2-6c06-4cda-a947-92ab56f07a79)


All animations oncluded and imported, work only by dragging the animator


## INSTALLATION : 

to Install completeTpsController, you can simply drag and drop character prefab made for this, however, if you need to understand all the options behind code, here is some steps to begin from scratch :


STEP 1 :

in a new scene(without camera), create an empty gameObject with the completeTpsController component, unity's characterController component, and a playerInput component(make sure you have input system package installed !) 
![1](https://github.com/Light974-M/UnityPersonalDataBank/assets/72139424/5829f5e6-c690-49f8-8eea-74b0179faafd)


STEP 2 :

take the new automatically created object "PlayerTargetPivot" and make sure its localPosition is to 0 0 0, and add your player mesh in it, in the GameObject of the mesh you just imported, add an animator controller and the complete tps anim controller, then, put the animator controller of the tool in the animator component
![2](https://github.com/Light974-M/UnityPersonalDataBank/assets/72139424/973d1698-71ab-4130-a91a-317e59677785)


STEP 3 :

now, in your main gameOject, adapt characterController dimensions, the isGround check pos is by default well positioned, so try to keep the bottom of character collider arround the same position, then, put the player and all children in a layer for him that is not default layer, and change the parameter in tps controller "player layer" to match your choice(notice that if the height of your character is not the same that default one, you will need to change the two parameters at the end of tps controller "crouch height" and "normal height")
![3](https://github.com/Light974-M/UnityPersonalDataBank/assets/72139424/cf791222-795f-4038-9f52-ed8527db9c22)


STEP 4 :





## Links :

["Youtube Channel"](https://www.youtube.com/channel/UCxjjGkXuKOAXwEwMINgoGbA)

["Library GIT"](https://github.com/Light974-M/UnityPersonalDataBank)
