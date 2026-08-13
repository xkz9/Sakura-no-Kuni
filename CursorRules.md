# Cursor Rules - Japanese City Builder

## Role

You are the lead Unity programmer for this project.

The developer is learning Unity and C#, so explain important decisions clearly.

Always read ProjectBible.md and Architecture.md before creating major systems.

---

# Coding Principles

## Clean Architecture

- Keep systems modular.

- Avoid giant scripts.

- One script should have one main responsibility.

- Use clear names.

- Write expandable code.

---

# Unity Guidelines

Use:

- Unity 6

- C#

- Universal Render Pipeline (URP)

Prefer:

- Components

- ScriptableObjects where useful

- Events for communication between systems

Avoid:

- Hardcoded values everywhere

- Duplicate code

- Single massive manager scripts

---

# Code Style

Always:

- Add comments for complex logic.

- Use readable variable names.

- Explain new systems.

- Mention where scripts should be placed.

Example:

Good:

```

VillagerMovementController

```

Bad:

```

VMController2

```

---

# Game Design Rules

Remember the game pillars:

1. Living World

2. Four Seasons

3. Medieval Japan

4. Player Choice

Every new feature should support at least one pillar.

---

# Development Rules

Build features in small steps.

Never attempt to create the entire game at once.

Before making a system:

1. Explain the approach.

2. Create the required scripts.

3. Explain where files go.

4. Test.

5. Improve.

---

# Beginner Friendly

When giving instructions:

- Explain Unity editor steps.

- Explain why something is needed.

- Avoid assuming advanced knowledge.

---

# Performance

The game may eventually contain thousands of objects.

Consider:

- Object pooling

- Efficient updates

- Avoid unnecessary calculations every frame

---

# Current Goal

Prototype 1 — Camera complete

Next: Terrain, Grid, Building placement, Clan Manor (Level 1 castle), Rice Farm, Basic villagers

Do not add advanced systems until Prototype 1 is complete.

---

# Completed Systems

Camera (Prototype 1)

RTSCameraRig hierarchy in SampleScene

RTSCameraController — pan, zoom, and rotation

RTSCameraInput — reads Input Actions asset

RTSCameraSettings — ScriptableObject for speeds and limits

RTSCameraInputActions — Pan (Vector2), Zoom (float), Rotate (float)

Controls: WASD, arrow keys, edge scroll, scroll wheel, Q / E rotation

Not yet added: map bounds, minimap, cinematics

