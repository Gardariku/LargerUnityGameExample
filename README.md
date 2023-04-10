# LargerUnityGameExample
Here I'm trying to implement more "serious" game structure in unity, using conventional frameworks and technologies. Aim is set for an easily modifiable and expandable systems architecture.
___
Actual gameplay isn't really there at the moment, so here's a small description about what's happening (and supposed to happen in the future) and why:

Main idea is to implement some sort of ***roguelike with tactical turn-based fights and procedurally generated HOMM-style grid map***. Cince roguelikes strong side is their variety,
game systems should allow to easily implement any kind of an idea for new enemy, skill, item, interactable object, etc., with all kinds of crazy effects. <br>
In order to do that all interactable objects on the <strong>world map</strong> are configured as combination of some _trigger_-script and a set of abstract <em>world events</em>, 
whose implementation defines objects behaviour. These events are also split into 2 packs: these same _events_, and their _consequences_. 
This way you can change behaviour depending on players choice or result of his actions (let's say event offers choice of getting an item or teleporting to a new level 
and then consequence-event according to players choice is fired). <br>
This allows not to only reuse existing behaviour, but to also create all types of their combinations for a new type of interaction.

Then the **battle systems** (like abilities and stats) are also designed in that manner, though it's hard to evaluate yet. Ability is also a combination of 
separate <em>effects</em>, which can be then recombined to create a new ability. Then there are also _statuses_(like temporal and permanent buffs/debuffs 
or just additional rules), which behave in a similar manner (combination of effects), but are not used directly and instead are called in reacion 
to their respective _trigger-events_, so there you also combine sets of effects with different triggers.<br>
Maybe one more thing to notice is that main part of battle logic is supposed to be implemented in those _effects_ (made with command pattern btw) and statuses, 
but that's currently not really true and there's a lot to decide about gamedesign.
___
**Procedural world generation** is split into two parts:
<ol>
<li> _Cellular automata_ is used to generate "skeleton" of the map, i.e. impassable obstacles.</li>
<li> _Wave function collapse_ then populates free space with interactable objects. 
This might not be the best choice, i just wanted to try this method, will see how it goes.</li>
</ol>
