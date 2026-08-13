# Project Bible

# Japanese City Builder

Version: 0.1

Status: Prototype 2 — World Foundation (In Progress)

---

# 1. Game Overview

## Working Title

Japanese City Builder

## Genre

Medieval Japanese city builder / kingdom simulator

## Core Idea

The player starts as the leader of a small Japanese clan with only a few families.

The settlement begins around a small fortified manor — the heart of Sakura no Kuni and the centre of the player's clan.

Over generations, the player grows a small village into a powerful province by managing:

- Buildings

- Resources

- Families

- Seasons

- Politics

- Trade

- Technology

- Diplomacy

The goal is not only to build a large city, but to create a living world where every family, decision and event creates stories.

---

# 2. Game Vision

The player should feel like:

"I am not just placing buildings. I am guiding a living kingdom through history."

The world should feel alive:

- Children grow into adults

- Families continue for generations

- Villages change naturally

- Seasons affect everyday life

- Political decisions have consequences

---

# 3. Main Inspirations

Games that inspire this project:

- Kingdoms and Castles

- Manor Lords

- Banished

- Foundation

- Cities: Skylines

- Total War

This game should take inspiration from these games but create its own identity.

---

# 4. Core Design Pillars

## Pillar 1: Living World

Every person matters.

Villagers have:

- Names

- Families

- Jobs

- Skills

- Relationships

- Ages

The player should become attached to their people.

---

## Pillar 2: Four Seasons

The year is divided into:

## Spring 🌸

Effects:

- Rice planting

- More rainfall

- Festivals

- Increased happiness

---

## Summer ☀️

Effects:

- Fast crop growth

- Fishing bonuses

- Heat problems

- Possible droughts

---

## Autumn 🍂

Effects:

- Harvest season

- Food storage

- Festivals

- Preparation for winter

---

## Winter ❄️

Effects:

- Farming slows/stops

- Food consumption increases

- Snow affects movement

- Survival becomes difficult

---

## Pillar 3: Medieval Japan

The game should feel authentically inspired by medieval Japan.

Features:

- Castles

- Samurai

- Rice fields

- Temples

- Shrines

- Tea houses

- Bamboo forests

- Traditional architecture

---

## Pillar 4: Player Choice

There should not be one correct way to play.

Players can focus on:

- Trade

- Military strength

- Religion

- Farming

- Diplomacy

- Expansion

---

# 5. Gameplay Loop

Gather resources

↓

Expand from the clan manor

↓

Attract families

↓

Create farms and industries

↓

Grow population

↓

Develop technology

↓

Manage politics

↓

Expand territory

↓

Become a powerful clan

---

# 6. World Simulation

The world contains:

- Mountains

- Forests

- Rivers

- Coastlines

- Plains

- Villages

- Other clans

The map should feel natural and different every playthrough.

Worlds are procedurally generated — not hand-made. See **Procedural World Generation** below.

---

## Procedural World Generation

Each new settlement begins in a unique generated world.

Every new game should generate a new world. Each world is created from a **seed** so the same world can be recreated.

The generator should eventually create:

- Terrain height and landscapes
- Mountains and valleys
- Rivers and lakes
- Forest areas
- Resource locations
- Starting castle location

The player's castle is the centre of the generated settlement.

The generator should find a suitable starting location:

- Enough flat land for the castle
- Surrounding buildable space for future settlement expansion (houses, farms, roads, and other buildings)
- Access to resources
- Interesting terrain nearby

### Generation API

WorldGenerator does not run automatically from Start().

A future Game Manager or Main Menu calls GenerateWorld(seed) when the player starts a New Game. Load Game passes a saved seed to recreate the same world.

For Prototype 2 testing only, an optional bootstrap helper may call GenerateWorld on Start — temporary, not core design.

### Data lifecycle

**WorldGenerationContext** — temporary data used only during the generation pipeline.

**WorldData** — represents the completed generated world (seed, terrain, castle origin, start location). Supports saving and loading later.

### Development approach

Do not attempt to build the full generator immediately.

**Prototype 2 (World Foundation)** should only create the foundation:

- Basic terrain generation
- Seed system
- Simple environment placement
- Placeholder castle placement

Keep future systems modular so they can expand later:

- Seasons
- Weather
- Biomes
- Resources
- Villages
- Roads
- Buildings

The RTS camera system remains unchanged.

---

# 7. Resources

Starting resources:

Wood

Stone

Food

Rice

Water

Tools

Later resources:

Iron

Gold

Silk

Tea

Fish

Coal

---

# 8. Buildings

<a id="sakura-castle-update"></a>

## The Castle System

The castle is the heart of Sakura no Kuni. It replaces the idea of starting with a simple house.

The player begins with a small fortified manor that represents the centre of their clan and settlement. The castle should visually and functionally grow over time through upgrades.

Castle progression:

**Level 1 — Clan Manor**

- Small fortified residence
- Wooden palisade
- Basic gate
- Starting point of the settlement

**Level 2 — Fortified Manor**

- Stronger walls
- Improved gate
- Larger courtyard
- More storage and administration

**Level 3 — Castle**

- Stone walls
- Towers
- Defensive structures
- Represents a growing town and stronger clan

**Level 4 — Regional Stronghold**

- Larger castle complex
- More advanced facilities
- Greater political influence

**Level 5 — Great Castle**

- Large multi-tiered castle
- Symbol of the clan's power
- Centre of the entire region

Design principles:

- Castle upgrades should not only increase strength — they should unlock new gameplay systems
- The castle represents the player's progress and prestige
- The castle should visually change as it levels up
- Seasonal changes should affect its appearance:
  - Spring: cherry blossoms and fresh vegetation
  - Summer: lush greenery
  - Autumn: red/orange leaves
  - Winter: snow-covered roofs and grounds

Future systems connected to castle level:

- Clan leadership
- Politics
- Diplomacy
- Taxes
- Samurai recruitment
- Storage
- Administration

---

Early buildings:

- Clan manor (Level 1 castle — player starting building)

- Small houses

- Rice farms

- Woodcutter

- Storage

- Well

- Market

Mid-game:

- Temples

- Schools

- Blacksmiths

- Trading posts

- Defensive walls

Late-game:

- Great Castle (Level 5)

- Palaces

- Universities

- Large temples

---

# 9. Families System

Families are individual simulated groups.

Each family has:

- Name

- Members

- Age

- Occupation

- Skills

- Happiness

- History

Generations continue over time.

---

# 10. Politics

The player manages relationships between groups.

Groups:

## Farmers

Want:

- Low taxes

- Good farmland

- Safety

## Merchants

Want:

- Trade

- Markets

- Roads

## Samurai

Want:

- Honour

- Weapons

- Strong leadership

## Monks

Want:

- Temples

- Peace

- Education

---

# 11. Clans

Other AI clans exist.

Players can:

- Trade

- Ally

- Compete

- Spy

- Fight

- Negotiate

---

# 12. Technology

Technology unlocks:

- Better farming

- Stronger buildings

- Military improvements

- Trade improvements

---

# 13. Art Style

Target style:

Stylized low-poly medieval Japan.

Visual themes:

- Cherry blossoms

- Wooden buildings

- Lantern lighting

- Mountains

- Rice terraces

- Snow-covered villages

---

# 14. Audio Style

Music:

- Traditional Japanese inspired instruments

- Calm village themes

- Dramatic war themes

Sounds:

- Rivers

- Wind

- Birds

- Villagers

- Blacksmiths

- Markets

---

# 15. Development Roadmap

## Prototype 1

Goal: RTS camera for exploring the world

Features:

✓ Camera movement

WASD and arrow key pan

Mouse edge scroll

Scroll wheel zoom

Q / E rotation

RTSCameraRig hierarchy

RTSCameraController

RTSCameraInput

RTSCameraSettings

RTSCameraInputActions

---

## Prototype 2

Goal: World Foundation — first playable generated environment

Features:

⬜ Basic procedural terrain generation

⬜ Seed system (generate and recreate worlds)

⬜ Simple environment placement (trees, rocks)

⬜ Starting castle area (suitable flat land + placeholder manor)

⬜ Ground materials and lighting

⬜ Water (rivers or lakes)

Do not build the full generator yet. Foundation only.

---

## Prototype 3

Goal: A tiny playable village

Features:

⬜ Grid system

⬜ Building placement

⬜ Clan Manor (Level 1 castle)

⬜ One farm

⬜ Basic villagers

---

## Prototype 4

Add:

⬜ Resources

⬜ Jobs

⬜ Storage

⬜ Population growth

---

## Prototype 5

Add:

⬜ Seasons

⬜ Families

⬜ Happiness

---

## Prototype 6

Add:

⬜ Politics

⬜ Clans

⬜ Diplomacy

⬜ Castle upgrades (Levels 2–5)

---

# Current Development Stage

Prototype 1 — Camera complete (pan, edge scroll, zoom, rotation)

Prototype 2 — World Foundation (in progress)

Procedural terrain generation, seed system, environment placement, and starting castle area.

Next: Basic terrain generator with seed support