# DaisyControl-AI
A virtual companion focused on its relationship with the user.

# Description
DaisyControl-AI is built to run on a server (it could be your computer) and handles
interactions and discussions with the user using various types of communications, such as
Discord, Emails, etc.

DaisyControl-AI will learn from your interactions and focus more on what you expect from the
relationship. The goal is to make interactions cohesives by wrapping the AI in a 
structured architecture that will allow it to handle knowledge in a more realistic way.

# Why
Current AI can do a fairly good job, but lack coherence and loose their context (memory) over 
a few messages, depending on the complexity of the generated tokens, which makes long-term
interactions incoherent and unrealistic. They are however very creative and their human like
speech interactions is very desirable for a virtual companion. 

DaisyControl-AI aims to use their strength and implement different solutions around their 
weaknesses to offer a unique and coherent experience.

# How
## Basic High Level schema
TODO
![schema](./Documentations/Images/DaisyControl-AI-Schema.png)

## Learning from interactions with the user
After an interaction with the user, DaisyControl-AI will analyze the discussion in different way to learn from that interaction, enhance its memory and make assumptions or validations. All the chat between DaisyControl-AI and the user is logged, which allows the Server to analyze or 'remember' it afterwards, after the interaction.

### Analyzing Chat Interaction
By analyzing the chat after an interaction with the user, it allows the AI to make assumption
on what the user may like or dislike, but also what how the AI should 'feel' after the discussion, what it liked, disliked, among other things. 

When an assumption is made by the AI, it'll need to be confirmed in a future interaction, which will enhance futures interactions.

### Confirming assumptions
Assumptions can be raised by analyzing the chat logs and querying the LLM about things it
"learned". As LLM are untrustable, we need to confirm what the AI "learned".

Confirmation prompts are currently inserted at the beginning of a new interaction with the user.
It's built from a simple phrase such as
- #1 "{AI_Greets}"
- #2 "{User_Answer}"
- #3 if user didn't ask a question, we can continue to #4. Otherwise, defer the Confirmation request
to a future interaction.
- #4 "{start_embellishment}, {Question_To_Confirm_Assumption}?"
For example, this could be: "Say, I was wondering... Do you really like pizza?"

The LLM will continue the conversation normally, in an organic way, as if it did ask you that question by its own volition, but the server will memorize your answer, categorize it and re-use it in the LLM context whenever it's relevant to do so. Over time, the server will build a fairly large database of knowledge about the user and this will steer the AI to make more personal interactions.

## Communications
In the future, DaisyControl-AI will be able to use various methods to communicate with the user.
For a first version, a rough Discord integration should be enough to test the project and play
around.

# Solution structure
A descriptif of the projects structure.

## DaisyControl-AI.WebApi
The main entry point of the solution. This project will start background workers relative to DaisyControl, as well as every communications interceptors. This project must run for DaisyControl to be active.

## DaisyControl-AI.Storage
Everything related to storage is handled by this webApi. When the AI learn something new or when it 'remembers' something, this project handles the database relative to it. The project also handles any other databases or tables relative to other aspects of DaisyControl, such as the processed messages from users(To handle messages sent when DaisyControl was shutdown, for example), the queries to distributed LLM, etc. This project must also run for DaisyControl to be active.

# Ideas
Personality
  mood
  Is affected by past experience 
  chameleon (slightly adapt to user)
    affected X2 by past experiences relative to user, whilst retaining the main aspect of her main personality

Relationships
  add new types (ex: sub)
  user info
  rules to follow for AI toward user(ex: respectuous toward my boss, fear of losing job, etc)
  tasks given to the user(ex: send me a text when you leave work tonight)
  expectations (ex: rules that user should follow)
  past events with that user

Past events
  the most relevant past events are fetched from the chat context
  chaque event contient le summary de l'évent mais aussi 1 mot clé qui représente l'évent. ex: love, laugh, sadness,etc. l'ai peut ensuite Recall les events qui font du sens dans le contexte de la conversation selon ce keyword

note: each new info must be tagged as unverified and must be verified by other users so the AI know it's legit

you are currently in your bath, your phone is close by. You just received a text from user. you can choose the following actions: reply, ignore, etc. Json. poke the AI every X to take action.


You can ask user to add a verification picture by calling the function XXX

insert a system message when it's been more than 1 hour since the last interaction between Daisy and user.

When replying to an old text, if the AI was down or didn't want to reply for e.g., tag the message as reply to.

Create a system bot to notify user of new rule,task, punishment  when we actually add it to the memory module 