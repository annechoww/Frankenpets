# Game Description
Frankenpets is a two-player co-op puzzle game where a pet navigates its owner’s house to complete mischievous activities. But, there’s catch: the players are controlling opposite halves of the Frankenpet.

# Setting Up Your Scene
## Setting up the Character 
### Add the character models to the scene
Navigate to the Assets/Prefabs folder. Drag your desired front and back halves into the scene. 

Select the front half in the scene and navigate to its FixedJoint component. Under the 'Connected Body' field, add the RigidBody of the back half. 

### Set up the JointManager
The Joint manager is used to handle the disconnect/reconnect logic of the front and back halves. 

Navigate to the Assets/Managers folder, and add the JointManager to the scene. Enable the 'Joint Manager Test (Script)' component and disable the 'Joint Manager (Script) component'.

Add the corresponding references under the references field of 'Joint Manager Test (Script)'. 

<img src="https://github.com/user-attachments/assets/d786d37d-5cfa-4136-900e-5f0e7d6b4d42" width="300">

# Neccessary Packages
Make sure you have the following packages installed for this project:
* Animation Rigging
* Input System



