# Backlog Items - QFrame Hackaton 2018

## Server - Tim

- Load in json data en keep in memory

**TASKS**

- Validate spectator vs bot
    - Add bot to list of connected bots, spectator likewise
- Initialize bot / spectator
    - BOT: 
        - try to recognize bot
        - determine level to play, might already have played previous levels
        - notify bot that level starts
            - contain level info, like grid size, player start spot
        - send updates of chosen level to bot, e.g.
            - TILE_DATA
                - tileX, tileY, type (wall, player, key, door) 
        - detect bot dies / finishes level
            - DIES: stop updates to level, notify bot that level is stopped, notify level start again, send updates again
            - FINISH: stop updates to level, notify bot that level is stopped,
            choose next level, start that level and sends updates
        - bot needs a way to react to updates
    - SPECTATOR: 
        - determine mode of spectating
        - instruct to spectate
 - Run game loop thread per room
     - every tick, a batch of updates is sent to all bots in the room
     - until next tick accept actions from bots, determine updates to send next tick
     - a move_down action may have a delay of 1 tick before executing and 1 tick cooldown before a new action is accepted

## Client - Bart & Niels

Common code that provides a level renderer to spectator and bot

### Spectator

1. Spectator starts
2. Connects to server
3. Server pushes instruction to host 'splash screen' or 'spectator mode'. in case of spectator mode, server specifies how to spectate: an array of rooms to display, whether to track player or keep full view

### Bot

1. Bot starts
2. Connects to server, waits until server pushes level
3. Server pushes information on first level
4. Bot starts level, server starts streaming updates

- Determine API that bot is coded against.

```
run("Team Tim", (updates, actions) => {
    updates.subscribe(update => {
        actions.moveUp();
    })
})
```

## Admin - Lennart

- Show list of currently connected bots
    - Kick a bot
    - Show which bots have reached which level
    - Show player information

- Show list of currently connected spectators
    - Change mode between:
        - Splash screen (mainly for start of hackaton, to let players team up / get explanation / setup env)
        - Spectating 

- Show list of levels
    - Configure a level
        - Unlock / lock