# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Olympic Manager MVP — a WebGL game on GitHub Pages. Player manages a fantasy athletic team and watches a 3D replay of a simulated Dragon Egg Relay event. Unity project targeting WebGL, no backend.

## Architectural Non-Negotiables

These rules are absolute. Never violate them:

| Rule | Constraint |
|---|---|
| Simulation owns truth | Replay consumes output — never rolls dice |
| All state serializable | Plain C# objects — no MonoBehaviour state |
| No SceneManager outside SceneFlow | All transitions go through SceneFlow |
| No Animator outside AthleteAnimator | All animation calls through AthleteAnimator |
| SceneFlow is routing only | No save logic, no parameters, no game state access |
| Frames are positions only | ReplayFrame has no AnimState |
| EventConfigData is pure C# | EventConfig (ScriptableObject) wraps it |
| No EventType string literals | Use `EventTypes.*` constants from Legends.Data |
| Assembly deps are one-way | Data ← Services/Simulation ← Replay/UI |
| GameSession is the scene handoff | Set LatestResult before navigating; ReplayDirector reads it on Start |

## Deferred — Do Not Build

Training, recruitment, seasons, equipment, economy, multiple events, physics-based egg handling, character creation, online services, analytics, multiplayer, Addressables, ECS, DI frameworks.

If any proposed implementation requires these: reject it.

## Assets

Synty packs live at `Assets/Synty/` (not `Assets/Asset Packs/Synty/`). Always check here before creating placeholders.

### Synty Asset Catalog

#### `PolygonFantasyKingdom/` — Castle, town, and kingdom environment (most relevant for the relay arena)
- **Castle/**: Walls, towers, battlements (6 styles), archways, gates, drawbridge, catapult, moat, keep, chapel
- **Buildings/**: Houses (multiple styles), tents (7 variants), destroyed/burnt buildings
- **Environment/**: Cliffs, hills, trees, grass, flowers, rocks, river pieces, roads, bridges
- **Props/**: Chests, barrels, market stalls, carts, banners, flags, torches, fences, wells
- **Characters/**: Prince, Princess, Queen, King, Priest, Soldier (M/F), Rider (with horse)
- **Weapons/**: Swords, shields, spears, bows, crossbows, catapult projectiles
- **FX/**: Catapult fire barrage, candle flames, chandeliers, FX spheres

#### `PolygonDungeon/` — Dungeon interiors, enemies, and magic
- **Characters/**: Ghost (2), Goblin (M/F/Shaman/WarChief/Warrior×2), Hero Knight (M/F), Rock Golem, Skeleton (Knight/Slave/Soldier×2), Tormented Soul
- **Props/**: Barrels, beds, benches, bookcases, braziers, candles/chandeliers, cauldron, chains, chests, coffins, doors, floors/walls/pillars, traps (spike/acid/flame), torture devices, stairs, cages
- **Items/**: Potions (9 variants), coins (4), books, jars, vials, horn, shrunken head, mushrooms
- **FX/**: Candle flame, electricity, fire (2), ghost dust, glow, light ray, magic swirl, ring, skeleton spawn, sparkles, spiral, sword fire, trap acid/flame

#### `PolygonDungeonRealms/` — Outdoor dungeon world, ruins, dwarves, lava
- **Characters/**: Dwarf Soldier (M/F), Dwarf Worker, Hero Female, Nomad (M×3/F×2), Skeleton (3), Undead Knight
- **Buildings/**: Tents (round/standard), Dwarf buildings (3), Ruin arches/bridges/pillars/towers/walls
- **Environment/**: Bone ribs, brambles, crystals, mushrooms, cave rocks, lava terrain pieces
- **FX/**: Candle flame, cart rail sparks, dust/embers, electrical, falling dust, gold sparkle, hell brazier fire, floating rocks, lava bubbles/rocks

#### `PolygonDungeonMap/` — Library and map room props (dungeon DLC)
- Books (5 types + piles/open/stacks), bookcases (standard/grand/small, broken/empty variants), globes, ladders, papers (8 variants), tombs (standard/royal + lids), knight stands, book stands, balcony/railing tiles

#### `PolygonFantasyCharacters/` — Fantasy NPC civilians and magic users
- **Characters/**: Female (Druid, Gypsy, Peasant×2, Queen, Witch), Male (Bard, King, Peasant, Rogue, Sorcerer, Wizard)
- **Props/**: Broom, bucket, crystal ball, dagger, druid staff, jug, lute, sceptre, spellbooks (3), ornate sword, vials, wand, wizard staff

#### `PolygonFantasyHeroCharacters/` — Fully modular rigged hero characters (M/F)
Animated skeletal characters with ~20 swap variants per body slot:
- Body slots: Head, HeadAttachment, Torso, ArmUpperL/R, ArmLowerL/R, Hand L/R, Hips, LegL/R
- Armor/clothing styles: Bare, light/medium/heavy armor, robes, ranger — each in male + female variants
- Entry point: `Prefabs/ModularCharacters.prefab` (demo rig); static parts under `Prefabs/Characters_ModularParts_Static/`

#### `PolygonFantasyRivals/` — Boss/villain characters with weapons
- **Characters/**: Ancient Queen, Ancient Warrior, Barbarian Giant, Big Ork, Dark Elf, Dwarf, Elemental Golem, Evil God, Forest Guardian, Forest Witch, Fort Golem, Mechanical Golem, Medusa, Mutant Guy, Mystic, Pig Butcher, Red Demon, Slayer, Spirit Demon, Troll
- **Weapons/**: Per-character signature weapon prefabs
- **FX/**: Fireball, energy pull/push, fire (circle/swirls), magic missile/swirl, runes, smoke, sparks, blood splat
- **Props/**: Character display bases (dirt/dungeon/grass/rock/mechanical), bones, mushrooms, skull, trees, pouches

#### `PolygonGeneric/` — Universal modular building kit + shared props
- **Base/**: Snap-together floor/wall/ceiling/roof/door/window/stair/pillar tiles (full, half, quarter, angled variants) — theme-neutral, used as building blocks for any setting
- **Building/**: Background buildings (11), beams, ladders, pipes (straight/corner/cross/T/valve)
- **FX/**: Blood splatter, candle flame, dust, fire, fog, leaves, rain, smoke
- *Note: Intended as foundation tiles that other packs build on top of*

#### `PolygonParticleFX/` — Standalone particle effects library
- Artillery shell/strike, explosions (body/large/dark), fireworks (group + color variants), blizzard/snow, blood splats, bubbles, cannon shot, cartoon footstep/jump — all as drop-in prefabs

---

#### `InterfaceFantasyWarriorHUD/` — Fantasy HUD UI kit (2D/UI layer)
**Prefabs** (drop-in UI components, all animated):
- `ActionBar/` — HotBar (3 variants), cooldown item slots
- `Compass/` — 3 bar variants; `Minimap/` — box (2), dial (4), diamond (2)
- `NPC_HealthBars_EnemyData/` — world-space enemy name/health, boss bars (2), damage toaster
- `Player_Health_Equipment/` — stat boxes, health/equipment panels
- `Popups_Notifications/` — level-up, quest complete, new location, saving, time-up, dialogue events
- `Objectives_Story_Location/` — objective header/items, dialogue response buttons
- `Input_Interactions/` — context-sensitive prompts (3), item pickup info (3), stat displays
- `Log/` — event log, chat log; `Reticles_Crosshairs/`; `WeaponWheel_WeaponCross/`
- `_PreMadeHUDs/` — fully assembled HUD layouts
- `_CommonComponents/` — labels (5 styles), button, slider (H/V), dial

**Sprites:** `Icons_Weapons/Resources/Inventory/Status/Map/Elements/Stats`, `FX/`, `Cursors/` (crosshair×5, pointer×5), `HUD/`

**Animations:** Full button-state clips + event notification in/idle/out clips for all event types.

---

#### `InterfaceCore/` — Input prompt sprites only
- `Icons_Input/` subfolders: GamepadGeneric, MouseKeyboard, PlayStation, Xbox, Switch
- Each icon: `_Clean` (foreground) + `_Underlay` (shadow) PNG variants

---

#### `SidekickCharacters/` — Modular skeletal human characters (meshes only, no animations)
All under `Resources/Meshes/` for `Resources.Load` at runtime.

**Species/Humans/** — 10 base bodies (`SK_HUMN_BASE_01`–`10`), each with: hair, eyebrows, ears, face, nose, teeth, tongue

**Outfits/FantasyKnights/** — 3 knight sets (`SK_FANT_KNGT_01`–`03`): torso, upper/lower arms, hands, hips, legs, feet, helmet (front/back/sides), back plate, shoulder/elbow/knee pads

Always check `Assets/Asset Packs/Synty/` for models, animations, effects, and UI elements before creating placeholders. See memory for details.

