# DaisyControl-AI
A virtual companion focused on BDSM relationship with the user.

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
![alt text](./Documentations/Images/DaisyControl-AI-Schema.png)

## Learning from interactions with the user
After an interaction with the user, DaisyControl-AI can analyze it in different way to learn
from that interaction, build memory and make assumptions or validations. All the chat between
DaisyControl-AI and the user is logged, which allows the Server to analyze it afterwards.

### Analyzing Chat Interaction
By analyzing the chat after an interaction with the user, it allows the AI to make assumption
on what the user may like or dislike, among other things. When an assumption is made by the AI,
it'll need to be confirmed in a future interaction.

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
For example, this could be: "Say, I was wondering... Do you really like pizza? [Start your answer with Yes/No]"

The LLM will continue the conversation normally, as if it did ask you that question by itself,
but the server will memorize your answer, categorize it and re-use it in the LLM context
whenever it's relevant to do so. Over time, the server will build a fairly large database of
knowledge about the user and this will steer the AI to make more personal interactions.

## Communications
In the future, DaisyControl-AI will be able to use various methods to communicate with the user.
For a first version, a rough Discord integration should be enough to test the project and play
around.