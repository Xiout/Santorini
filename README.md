# Santorini
Unity version of the board game "Santorini"

Santorini is a competitive board game for 2 to 3 players with very straighforward base mechanics.
The board consists of 25 cells (5x5)
Each players has 2 paws called "Builder"
During the game, players are going to build towers all over the board. Each tower can have 3 floors and a roof. When the roof is build, nobody can move on this cell anymore.
The goal of the game is to place one of one's builder on a 3-floors tower

At their turn, the player must perform this 2 actions in this order :
*-Move one of their builders on a neighboring cell
*-Build on a neighboring cell with the builder they just moved

Note that :
*-A builder cannot move or build on top of another builder
*-A builder cannot go up more than 1 floor at a time

In addition of the base mechanics, there is also God card to spice up the game.
Each God card has a power that modify the rule of the game for one specific player.
Rules can offer additional movement or build, give constraint to opponant or even modify base mechanics or win condition.

For more information, please check the PDF of the rules (pages 1-3):
http://boardgame.bg/santorini%20rules.pdf

Current state of the project :
*-Basic mechanics implemented both for 2 and 3 players playing on the same computer
*-Temporary visual assets
*-Visual aide to define which builder is selected and where a builder can move or build
*-Option menu to change player's color and god
*-6/28 God Power implemented
