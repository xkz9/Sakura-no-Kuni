# Architecture
# Japanese City Builder

Version: 0.1

---

# Purpose

This document defines the architecture of the game.

Every new system should fit into this structure.

Goals:

- Modular
- Easy to understand
- Easy to expand
- Beginner friendly
- Performance conscious

No system should become a "God Object" that does everything.

---

# High-Level Architecture

Game
│
├── Camera
├── World
│   ├── WorldGenerator
│   ├── TerrainGenerator
│   ├── EnvironmentGenerator
│   ├── WaterGenerator
│   └── CastlePlacement
├── Grid
├── Buildings
├── Resources
├── Villagers
├── Families
├── Seasons
├── Politics
├── Clans
├── Economy
├── Events
├── UI
├── Audio
├── Save System
└── Utilities

---

# Camera

Purpose

Allow the player to explore the world.

Prototype 1

✓ RTSCameraController

✓ RTSCameraInput

✓ RTSCameraSettings

✓ RTSCameraInputActions

✓ RTSCameraRig hierarchy

Pan (WASD, arrow keys, edge scroll)

Zoom (scroll wheel, min/max height)

Rotation (Q / E keys, rotates RTSCameraRig on Y axis)

Future

Map Bounds

Minimap

Cinematics

---

# World

Responsible for:

Terrain

Trees

Rivers

Mountains

Weather

Lighting

Day/Night

## Procedural World Generation

Each new settlement begins in a unique generated world.

Every new game generates a new world from a **seed** so the same map can be recreated.

The generator should eventually create:

- Terrain height and landscapes
- Mountains and valleys
- Rivers and lakes
- Forest areas
- Resource locations
- Starting castle location

The player's castle is the centre of the generated settlement.

The generator finds a suitable starting location:

- Enough flat land for the castle
- Surrounding buildable space for future settlement expansion
- Access to resources
- Interesting terrain nearby

### Prototype 2 — World Foundation (current scope)

Do not build the full generator yet. Foundation only:

- Basic terrain generation
- Seed system
- Simple environment placement (trees, rocks)
- Placeholder castle placement

### Modular expansion (future)

Keep systems separate so they can be added later:

- Seasons
- Weather
- Biomes
- Resources
- Villages
- Roads
- Buildings

### Planned architecture

WorldGenerator (orchestrator — public API, not tied to Start)

├── GenerateWorld(seed) — called by Game Manager, Main Menu, or debug tools

├── WorldSettings (ScriptableObject — map size, generation parameters)

├── WorldGenerationContext (temporary — lives only during the pipeline)

├── WorldData (persistent result — seed, terrain refs, castle origin, start location)

├── TerrainGenerator (heightmaps, landscapes)

├── StartLocationFinder (scores flat land + surrounding buildable expansion space)

├── CastlePlacer (places manor placeholder at chosen location)

├── EnvironmentGenerator (trees, rocks)

└── WaterGenerator (rivers, lakes — future)

### Generation API

WorldGenerator does not generate in Start().

Instead it exposes:

- GenerateWorld(int seed) — create a world from a specific seed
- GenerateRandomWorld() — pick a random seed, then call GenerateWorld

A future Game Manager or Main Menu will call these when the player chooses New Game. Load Game will pass a saved seed to GenerateWorld(seed) to recreate the same world.

For Prototype 2 testing only, a small optional helper (e.g. WorldGeneratorBootstrap) may call GenerateWorld on Start — this is temporary and not part of the core design.

### Data lifecycle

**WorldGenerationContext** — temporary. Created at the start of GenerateWorld, passed through each pipeline step, discarded when generation finishes.

**WorldData** — persistent. Created when generation completes. Holds everything other systems need after the world exists:

- Seed
- Reference to Terrain
- CastleOrigin position and rotation
- Start location world position
- Castle clear / buildable radius
- Reference to GeneratedWorld root transform

WorldGenerator stores the current WorldData (e.g. CurrentWorld) so Grid, Buildings, and Save System can read it later.

### Start location scoring

StartLocationFinder must think about future settlement growth, not just flat ground under the castle.

Scoring criteria:

- **Castle flatness** — low slope under the manor footprint
- **Expansion space** — large surrounding area suitable for future buildings (low slope, adequate radius)
- **Buildable coverage** — percentage of expansion zone that passes slope/height rules
- **Elevation** — mid elevation preferred; avoid peaks and valley floors
- **Interest** — hills, forest, or water within view distance

The chosen spot should leave room for houses, farms, and roads in later prototypes without requiring terrain edits.

The RTS camera system is unchanged.

Future:

Terrain editing

Biome support

Full resource placement

---

# Grid

Responsible for:

Building placement

Cell occupation

Grid highlighting

Placement validation

Road connections

Future:

Different terrain costs

Bridge support

Height differences

---

# Buildings

Every building shares common behaviour.

Base

Building

Derived Examples

House

Rice Farm

Market

Storage

Temple

Castle (Clan Manor → Great Castle)

Blacksmith

## Castle System

The castle is the heart of Sakura no Kuni and the player's starting building — not a simple house.

Progression: Clan Manor → Fortified Manor → Castle → Regional Stronghold → Great Castle

Each level upgrades visually and unlocks new gameplay systems (politics, diplomacy, taxes, samurai recruitment, storage, administration).

Seasonal appearance changes: cherry blossoms (spring), lush greenery (summer), autumn leaves, snow-covered roofs (winter).

Future

Upgrades

Damage

Repairs

Production

Building modifiers

---

# Resources

Handles:

Wood

Stone

Rice

Food

Iron

Tea

Silk

Gold

Coal

Fish

Future

Luxury resources

Rare resources

---

# Villagers

Every villager exists as an individual.

Contains

Name

Age

Job

Family

Current task

Needs

Happiness

Future

Skills

Memories

Relationships

Schedules

---

# Families

Responsible for

Marriage

Children

Deaths

Inheritance

Family trees

Population growth

Future

Family reputation

Clan influence

Generational history

---

# Seasons

Spring

Summer

Autumn

Winter

Controls

Weather

Crop growth

Festivals

Movement speed

Food usage

Visual changes

Future

Natural disasters

Climate

---

# Politics

Groups

Farmers

Merchants

Samurai

Monks

Nobles

Controls

Taxes

Influence

Approval

Laws

Future

Civil wars

Political events

---

# Clans

AI Kingdoms

Relationships

Trade

Diplomacy

Expansion

Wars

Future

Marriage alliances

Espionage

Negotiations

---

# Economy

Controls

Markets

Trade

Prices

Supply

Demand

Future

Inflation

Merchant caravans

Imports

Exports

---

# Events

Random world events

Examples

Harvest festival

Fire

Flood

Earthquake

Bandits

Travelling merchant

Famous swordsman

Future

Story events

Historical events

Quest chains

---

# UI

HUD

Resource bar

Build menu

Population

Notifications

Settings

Future

Statistics

Graphs

Minimap

---

# Audio

Music

Ambient

Weather

Building sounds

Villagers

Animals

Future

Dynamic music

Regional themes

---

# Save System

Responsible for

Saving

Loading

Auto saves

Version compatibility

Future

Cloud saves

Multiple save slots

---

# Utilities

Shared systems

Math

Extensions

Helpers

Debug tools

Pooling

Future

Performance profiling

Developer console

---

# Development Rules

Every system should:

Have one responsibility.

Avoid duplicate logic.

Be easy to expand.

Be documented.

Be beginner friendly.

---

# Script Naming

Use descriptive names.

Good

VillagerMovement

BuildingPlacement

GridManager

SeasonManager

FamilyTree

Bad

Manager2

Script3

TestScript

---

# Folder Structure

Assets

Scripts

Camera

RTSCameraController

RTSCameraInput

RTSCameraSettings

World

WorldGenerator

WorldSettings

WorldData

WorldGenerationContext

TerrainGenerator

StartLocationFinder

CastlePlacer

EnvironmentGenerator

WaterGenerator

Grid

Buildings

Resources

Villagers

Families

Politics

Clans

Economy

UI

Audio

Utilities

Prefabs

Materials

Models

Scenes

Textures

ScriptableObjects

Camera

RTSCameraSettings

World

WorldSettings

Input

RTSCameraInputActions

Audio

UI

Art

---

# Current Prototype

Prototype 1 — complete

✓ Camera (unchanged)

Prototype 2 — World Foundation (in progress)

Procedural terrain generation

Seed system

Simple environment placement

Starting castle area placement

Prototype 3 and later

Grid

Building Placement

Clan Manor (Level 1 castle)

Rice Farm

Villagers
