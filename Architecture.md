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

Future

Map Bounds

Rotation

Q / E keys

Rotates RTSCameraRig on Y axis

Add after Prototype 1 MVP is complete

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

Future:

Procedural generation

Terrain editing

Biome support

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

Castle

Blacksmith

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

Input

RTSCameraInputActions

Audio

UI

Art

---

# Current Prototype

Prototype 1

✓ Camera

Terrain

Grid

Building Placement

House

Rice Farm

Villagers

Nothing else should be developed until Prototype 1 is complete.
