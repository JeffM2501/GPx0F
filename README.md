#Whatever



#Notes

Ship has 2 resources, power and shields.

Power generates at a fixed rate and used by things.

Shields go down when damaged. At shield 0, ship explode.
Shields recharge slowly when they remain undamaged for some time and there is power available.
Player is warned when shields are below 25%. Sheild recharge power drain is less than generation rate.

Power can be used to boost speed (shift key). Boosting drains power faster than it can be generated.
Jumping uses power for the inital jump boost.

## Weapons
Ships have 2 main weapons, Primary and optional.
Weapons can be aimed up and down in a limited arc.
Primary weapon is a rapid fire blaster that uses a small ammount of power. Constat firing will drain power faster than it can be generated.
Optional weapons are picked up in game and activated with the alternate fire controll (right click).

* Cannon - 
	Has Ammunition, uses no power, shots are affected by gravity and explode on impact with splash damage.
* Plasma Blaster - 
	Uses Power, shots are not affected by gravity, small splash damage
* Tesla Field  - 
	Has ammunition (Charges), but also uses power when armed. User hold down fire button to arm field, this slowly drains power. 
	When a target is inside an armed field, a charge will be used to send a lightning bolt to the target, damaging it. Lighting can jump to nearby targets.
* Rockets - 
	Has ammunition, uses no power, fires dumb-fire rockets that explode on impact. Low damage per rocket, but there are lots of them.
* Missile - 
	Has ammunition, uses power to lock. User holds fire button to lock target (CTL for multiples). Power used while locking. Missile(s) launch when fire button is released.
	If fired with no lock, will look for a target after some arming time. Higher damage than rockets.
* Laser - 
	Uses Power, continuous beam that damages all in it's path.
	
Only one optinal weapon can be held at a time.

Ships can have one accessory in additon to a weapon. Accessories are picked up in game and activated using the 'q' key. All accessories take power in some way.

* Shield Booster - 
	Adds an overshield that takes damage first. Overshield will not recharge normaly. Accessory must be activated to recharge overshield, this process takes a lot of power.
* Jamming Field	 - 
	Cuasses in acurate radar and visual readings in some radius, takes power to activate. Cuses missles to loose lock and reaquire target (may aquire shooter).
* Turbo - 
	Increases boost speed and power drain
* Manuvering Thrusters - 
	Increases jump height and increases in air manuver speed. Uses power automaticaly while jumping.
* Hologram - 
	Sends out realistic looking images of ship and shots. Uses power while depolyed. Destoryed holograms depoete power faster (Feedback)
* X-Ray Sensors - 
	Shows the conents of all pickups, hides hollograms, prevents effects of Jamming Field. Shows markers for all nearby ships, even if behind objects.

Both weapoons and accessories are aquired from pickups in game. Pickups are anonymous untill aquired, so a player will not know what is in them unless they have active X-Ray sensors.
If a ship has an accessory and an optional weapon, driving through a pickup will do nothing. 

##Traps
Some pickups are trapped and will cause a ship malfunction. Trapped pickups will not give a weapon or accessory, but are visibile to X-Ray Sensors.
All traps wear off after some randomized time as the ship reboots systems. Weapons and accessories can not be dropped until a trap's effects are repaired.

Trap Effects
* Generator Failure
	Reduces power generation
* Shield Failure
	Reduces shields by 50% (will not go to 0), and reduces recharge time.
* Thruster Malfunctions
	Reduces speed and manuvering
* Sicky Throttle
	Foces an imediate overboost in the forward direction that can not be stopped by the pilot. Power will be reduced to 10% after overboost is repaired.
* Sensor Malfunctions
	Ship has effect of being in a jamming field at all times, even overrides X-Ray Sensors.
* Fire Controll Mallfunctions
	Weapons will fire at random times.

## CTF
CTF works like standard FPS CTF. Each team has a base and the flag spawns at the base. Flags at bases can only be grabbed by opposing teams.
Grabbing a flag drops any equiped optional weapon and accessory. Carrying a flag grants small speed boost and reduces shield recharge delay. Flag carriers can drop flag at any time.
Flag auto-drops when carrier is killed. Flags on the ground can be piked up by other opposing team members and continue to be carried. If a team membmer touches there own flag, 
the flag is instantly transported to it's base. If a flag is taken to an opposing teams base, and touches that team's flag (team flag must be at base), a caputre point will be awareded, and the captured 
flag will be returned to its base.

	
