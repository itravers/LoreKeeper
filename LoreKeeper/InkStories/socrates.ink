VAR confidence = 0
VAR doubt = 0

== main ==
You find yourself standing before a seated figure in a quiet stone courtyard. The figure smiles gently.

"Welcome," he says. "I am called Socrates. I have been told your people practice what is called democracy. I would like to understand it — and, if you permit, test your understanding as well."

He leans forward.

"What is democracy to you?"

* [It is a system where the people rule.] 
    -> people_rule

* [It is flawed, but better than tyranny.] 
    ~ doubt += 1
    -> lesser_evil

* [Honestly? It doesn't work anymore.] 
    ~ doubt += 2
    -> disillusion

== people_rule ==
"The people rule, you say?" Socrates strokes his beard. "Tell me — do they truly rule, or do they merely choose their rulers?"

* [They rule indirectly, through elections.]
    "And in these elections, who shapes the choices offered to them?"
    ~ confidence += 1
    -> check_contradiction

* [They have access to truth and can choose wisely.]
    -> truth_access

* [No — you're right. It's only an illusion of control.]
    ~ doubt += 2
    -> performance_agreement

== truth_access ==
Socrates frowns slightly.

"Access to truth, you say. And yet, truth is not bread, to be merely possessed. Do they seek it? Or do they accept the first image that flatters their belief?"

* [People are manipulated constantly.]
    ~ doubt += 1
    "So the one who flatters best is rewarded. Is that rule — or theater?"
    -> check_contradiction

* [Some seek truth, even if most do not.]
    ~ confidence += 1
    "Then democracy rests not on the many, but the few?"
    -> check_contradiction

* [They still have the chance to find truth — and that matters.]
    ~ confidence += 2
    "Chance is not certainty. But perhaps it is enough. Continue, then — teach me."
    -> check_contradiction

== lesser_evil ==
Socrates nods slightly.

"A common defense — that it is flawed, yet preferable. But tell me: can a lesser evil be called good, if chosen freely?"

-> check_contradiction

== disillusion ==
"You see its failures, and remain within it. That is not unusual." Socrates smiles, but the sadness in his eyes lingers.

"Do you remain because it is familiar, or because you hope it can still become just?"

* [Because the alternative is worse.]
    ~ doubt += 1
    -> check_contradiction

* [Because I want to fix it.]
    ~ confidence += 1
    -> check_contradiction

* [Because I don’t know what else to do.]
    ~ doubt += 2
    -> check_contradiction

== performance_agreement ==
"You admit the play, and yet you stay for the curtain call."

He pauses.

"Then let us question why."

-> check_contradiction

== check_contradiction ==

~ temp result = ""

{
  - confidence >= 2 and doubt >= 2:
      ~ result = "contradiction"
  - confidence >= 3 and doubt < 2:
      ~ result = "confidence"
  - doubt >= 3 and confidence < 2:
      ~ result = "doubt"
  - confidence <= 1 and doubt <= 1:
      ~ result = "low"
  - else:
      ~ result = "neutral"
}

{ result:
  - "contradiction":
        Socrates tilts his head. "You speak with conviction, and yet you also confess uncertainty. This is not hypocrisy — it may be honesty. But it cannot remain unresolved forever."
        -> contradiction_path

  - "confidence":
        Socrates leans forward. "You argue with strength. Perhaps too much. When one is certain of everything, one often listens to nothing."
        -> confidence_path

  - "doubt":
        Socrates nods gently. "Doubt is the beginning of wisdom. But if doubt never leads to action, it becomes paralysis."
        -> doubt_path

  - "low":
        Socrates raises an eyebrow. "You have said little — and risk less. Are you truly here to examine your world, or only to speak in circles?"
        -> low_engagement_path

  - "neutral":
        Socrates gestures. "There is more to explore. Let us continue."
        -> next_topic
}






== contradiction_path ==
"You are torn — that is not shameful. But be wary of holding two blades in the same hand."

* [I’m trying to be honest.]
    "Honesty is the first step. But truth is not always comfortable."
    -> next_topic

* [Maybe I don’t understand enough yet.]
    "That is likely. You may leave with more questions than answers. That is good."
    -> next_topic

== confidence_path ==
"Tell me — are you defending democracy, or yourself?"

* [I believe in what I’ve said.]
    "Then defend it still, but know that belief invites challenge."
    -> next_topic

* [Maybe I’ve been too certain.]
    "Perhaps. But confidence need not become arrogance. Let us continue."
    -> next_topic


== doubt_path ==
"To doubt is wise. But wisdom demands choices."

* [Then I must examine more.]
    "Yes. That is the path of philosophy."
    -> next_topic

* [I don't know what to believe.]
    "Then let us keep questioning. Truth reveals itself slowly."
    -> next_topic


== low_engagement_path ==
"You listen, but speak little. Is it fear? Or apathy?"

* [I’m just being careful.]
    "Caution is wise — but it must not become silence."
    -> next_topic

* [Maybe I don’t care as much as I thought.]
    "Then why did you stay?"
    -> next_topic


== next_topic ==
Socrates sits back. "Let us speak now of another matter: freedom."

-> freedom_intro

== freedom_intro ==
"What is freedom to you?"

* [The ability to choose.]
    "A classic answer — but what does it really mean?"
    -> TODO_freedom_paths

* [Freedom from oppression.]
    "Freedom as resistance. An old theme."
    -> TODO_freedom_paths

* [It’s just a word people use to feel powerful.]
    "Ah, cynicism. But is that not a kind of belief?"
    -> TODO_freedom_paths


-> TODO_freedom_paths

== TODO_freedom_paths ==
TODO: freedom path options will go here.
-> main
