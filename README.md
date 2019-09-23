AI-Rollerball
Requires: HDRP, ml-agents

This project was to learn ml-agents in Unity.

The ball will roll to the glowy "jesus" cube.
To make changes, train RollerBallBrain.nn and set it as the model in the RollerBallBrain scriptable object.

To continue to train the existing nn, in an anaconda prompt with your config yalm in the default location, move the nn model to the models folder and type:

activate ml-agents
mlagents-learn config/trainer_config.yalm run-id=RollerBallBrain --load --train