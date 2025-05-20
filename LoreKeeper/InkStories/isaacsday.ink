// === GLOBAL STATE ===
VAR score = 0
VAR apartment_light_on = false
LIST inventory = car_keys, work_keys, wallet, phone  // All possible items
~ inventory = ()  // Start with no items

-> start


== main ==
->start

=== function show_inventory ===
Inventory:
{inventory:
  - car_keys: You have your car keys, they feel a little light.
  - work_keys: You have your office keys.
  - wallet: You carry a worn journal.
  - phone: A brass compass hangs from your belt.
  - else:
        You’re not carrying anything of note.
}
~ return


=== function examine_item(itemName) ===
{
  - itemName == "car_keys":
      ~ return "Your Car keys are just A small keyring with two keys on it."

  - itemName == "work_keys":
      ~ return "These are your office keys, why are they not on your main keyring?"

  - itemName == "phone":
      ~ return "Your cell phone is beat up and damaged."
  - itemName == "wallet":
      ~ return "This is a plain brown leather wallet."

  - else:
      ~ return "You turn it over in your hands, but there’s nothing remarkable about it."
}



== start ==
You are lying in bed. { apartment_light_on: The room is lit. | It's completely dark. } You're not sure what time it is.

{ apartment_light_on == false:
    + [Turn on the light]
        -> turn_on_light
}
+ [Get up]
    -> get_up
+ [Do nothing]
    You lay there for a moment longer. Nothing changes.
    -> start

== turn_on_light ==
~ apartment_light_on = true
The room is suddenly filled with soft yellow light. You squint against it. It's your apartment, just as you left it.

~ score += 1

-> start

== get_up ==
{ apartment_light_on == false:
    You sit up and immediately *crack* your head on something.
    
    "Ouch!"
    
    You fall back down, disoriented.
    
    ~ score -= 1
    
    -> start
}

{ apartment_light_on == true:
    "Well, I didn't get enough sleep, but I can't fall back asleep right now". You tell yourself.
    You struggle to get yourself out of bed
    
    ~ score += 1
    
    -> start_awake
}


== start_awake ==
You are now awake and aware. The day begins.

+ [Do My Morning Exercise]
    -> morning_exercise
+ [Check your phone]
    -> check_your_phone
* [Go to bathroom]
    -> go_to_bathroom
+ [Go to Front Room]
    -> go_to_frontroom
    

== morning_exercise ==

{~You return to your modest workout corner — a patch of carpet, a couple of dumbbells, and a dream. It’s not a gym, but it gets the blood moving and mildly impresses your furniture.|The space is small, the motivation is questionable, and your form is probably illegal in six states. But hey — you showed up.|You glance at your equipment. It sits there, smug and unused, like it’s daring you to do one rep without collapsing.|Back to the grind: you versus your own laziness. The stakes? Pride, sweat, and maybe a pulled muscle if you're lucky.}



* [Do sit-ups]
    You drop to the floor and start crunching. The carpet itches against your back as you push through each rep, your core burning by the fifth one. By the time you finish the set, you're breathing hard, but there's a faint satisfaction humming through your muscles.
    ~ score += 1
    -> morning_exercise

* [Do push-ups]
    You drop into push-up position with the confidence of someone who forgot how hard these are. The first few reps feel fine then your arms start trembling like spaghetti in a wind tunnel. By the tenth push-up, you're bargaining with gravity. Still, you power through, chest burning, and collapse in triumph. Not bad for a living room workout hero.
    ~ score += 1
    -> morning_exercise

* [Do curls]
    You grab your dumbbells — cold, worn metal biting into your palms — and start curling. Each rep grinds through your arms like rusted gears turning under strain. Your biceps burn, your breath sharpens, but you keep going. Pain is just weakness trying to negotiate its way out.
    ~ score += 1
    -> morning_exercise

+ [Check your phone]
    -> check_your_phone
    
* [Go to bathroom]
    -> go_to_bathroom
    
+ [Go to Front Room]
    -> go_to_frontroom

== fake_end ==
This is the END, my only friend, the end.
-> start

== check_your_phone ==

{~You unlock your phone and stare at the glowing screen like it's a cursed artifact. It's too early for this, but here we are.|You unlock your phone. The light hits your face like divine judgment. Too early. Too bright. Too bad.|You squint at your phone like it owes you money. It's too early for this, and yet you're scrolling.|You unlock your phone, hoping for wisdom or inspiration. Instead: 3 notifications and a low battery warning.|You wake your phone like a vampire greeting the sun. It sears your eyeballs, but you're committed now.}


+ [Check email]
    -> check_email

* [Check Facebook]
    You scroll past someone’s engagement, a political argument, three ads for socks, and a video of a dog baking cookies.  
    You're not sure how long you were in there, but your soul feels lighter... or possibly drained.
    -> check_your_phone

* [Check YouTube]
    You tell yourself you'll just watch one video.  
    One documentary, one cooking tutorial, and a conspiracy theory about pigeons later, you look up.  
    Time has ceased to exist.
    -> check_your_phone

* [Put the phone down]
    You sigh and put the phone face-down. Back to reality — whatever that is.
    -> start_awake
    
== check_email ==

{~Your inbox is a battlefield of promotions, reminders, guilt, and thinly veiled doom.|Your inbox looks like it lost a fight with capitalism, guilt trips, and bot-generated shame.|It’s a digital jungle in there — where coupons, crises, and your grandma’s judgment all fight for attention.|You scroll through a wasteland of updates, passive-aggressive messages, and at least one cursed chain letter.}


* [Check work email]
    Subject: **Change of Plans - Office Opening**  
    From: your supervisor  
    > "Hey, just a heads up — I'm running late this morning. You'll need to unlock the office. Don't forget your keys!"  
    You stare at the message, suddenly aware of the weight (or absence) of your keyring.  
    -> check_email

* [Check Viagra ad]
    Subject: **🔥 Restore Your Youth With One Pill 🔥**  
    From: some sketchy domain  
    > "Doctors hate him. Women love him. Click here for 900% results with no side effects!!"  
    You delete it immediately and then delete it again out of principle.  
    -> check_email

* [Check LoreKeeper notification]
    Subject: **Action Required: LoreKeeper Update**  
    From: Google Play Developer Console  
    > "Your app 'LoreKeeper' has not been updated in 90 days. Please update to meet API level 33 compliance or it will be removed."  
    You sigh. LoreKeeper always asks for more commitment than you can give before coffee.  
    -> check_email

* [Check email from Grandma]
    Subject: **Thinking of You**  
    From: Grandma  
     "I saw a young man on TV today doing very well in his career. He was giving a TED talk and looked so confident. It made me think of you. Not in a bad way — just in a wondering-what-happened sort of way.  
      
     I hope you're still working on that... app thing. Have you considered going back to school? Or maybe law enforcement — they have good benefits.  
      
     Your cousin Brandon just bought a house. With a *yard*.  
      
     Anyway, I’m proud of you, no matter what. But if you do figure things out soon, that would be wonderful.  
      
     Love, Grandma"  
    You feel 40% shame, 30% rage, 20% confusion, and just a whisper of love. Classic Grandma.  
    -> check_email


* [Check that random PayPal warning email]
    Subject: **Suspicious Activity Detected!!!**  
    From: paaypal-secure@payments.biz.ru  
    > "Your account has been locked due to irregular activities. Kindly login now or your funds will be incinerated."  
    You don't even have a PayPal account. Bold move.  
    -> check_email

+ [Return to phone menu]
    -> check_your_phone
    
== go_to_bathroom ==
{~You find yourself in the bathroom — small, humid, and lit by a flickering bulb that’s probably seen some things.|The bathroom greets you with its signature humidity, flickering light, and faint air of judgment.|Once again, you’re in the bathroom. It’s familiar, damp, and slightly unsettling.|You're back in the bathroom — or maybe you never left. Time blurs in here.|The bathroom hasn’t changed: still cramped, still flickering, still holding secrets you’d rather not confront.}



* [Take a shower]
    You turn on the water and wait a full minute before it stops being ice daggers.  
    You stand under the flow, letting it wash off sleep, stress, and probably some ambition.  
    You emerge feeling 12% cleaner and 3% more optimistic.  
    ~ score += 1
    -> go_to_bathroom

* [Use the toilet]
    A moment of peace. A porcelain throne. The only place where no one can judge you — except maybe the spider in the corner.  
    You do your business like a champ.  
    ~ score += 1
    -> go_to_bathroom

* [Leave the bathroom]
    {~You towel off, flush, or just decide you’re done. Either way, time to move on.|You stare into the mirror a little too long, then decide that’s enough introspection for one morning.|You dry off, suppress the urge to scream into the sink, and carry on like a champ.|You flush, wash your hands like it matters, and walk out feeling 3% more human.|You finish up and wonder if this counts as self-care. It probably does. Barely.|You dry off and pretend the bathroom isn’t the only part of your life that needs cleaning.|You leave the bathroom with damp skin, fogged glasses, and the determination of someone who’s pretending today will be different.}

    -> go_to_frontroom
    


== go_to_frontroom ==

{~You're in the front room again. Sunlight cuts across old dishes, unopened mail, and that same cluttered surface where your keys like to vanish.|The front room greets you with filtered light, yesterday’s dishes, a forgotten granola bar, and a vague sense that your keys are under something.|Sunlight sneaks through the blinds of the front room, illuminating a battlefield of mugs, junk mail, and the probable location of your keys.|The front room hasn’t changed: mess on the table, stale air, and a gentle whisper from the universe saying “your keys are probably here.”|Once more, you find yourself in the front room — coffee stains, unopened envelopes, and something metal glinting faintly beneath a pizza flyer.}


* [Eat breakfast]
    {~You scrape together a breakfast from whatever looks least expired. It’s not gourmet, but it counts.|You assemble a breakfast from toast crumbs, fridge mysteries, and blind optimism.|Breakfast consists of a granola bar, half a banana, and the shattered illusion of being prepared.|You cobble together a meal that would make a raccoon think twice — but hey, calories are calories.|You eat something that may once have been yogurt. You don't check the date.|You call it breakfast. Others might call it “emergency snacking under pressure.”|You stare into the fridge until breakfast manifests itself out of pity.}

    {~You feel slightly more human.|You feel 12% more functional and 3% less haunted.|You are now operating above goblin mode, if only barely.|You feel like a person who might someday return a phone call.|You feel marginally prepared to face a world that did nothing to deserve you.|You feel less like a cryptid and more like someone who owns socks.}

    ~ score += 1
    -> go_to_frontroom

* [Clean up]
    {~You gather up dishes, toss wrappers, and try to pretend you're someone with a handle on life. The room looks 12% better.|You clean with the energy of someone who might get a surprise visit from judgmental parents. It helps... a little.|You throw away the worst of the chaos and call it “good enough.” That counts, probably.|You clean just enough to feel morally superior to your past self.|You gather dishes and wrappers like you're prepping for a low-budget redemption arc.|You do a round of superficial cleaning and briefly feel like a functioning adult simulator.}
 
    ~ score += 1
    -> go_to_frontroom

* [Grab your keys]
    {~You find your keys under a magazine and a coupon for something you’ll never buy.|Your keys are wedged between an old takeout menu and what might be a receipt from 2019.|You unearth your keys like an archaeologist uncovering lost artifacts of questionable value.|Your keys turn up beneath a pile of junk mail and a deeply outdated dentist reminder.|You find your keys hiding under a pizza flyer and an unopened envelope labeled “URGENT” from two weeks ago.|There they are — your keys, tucked behind a coaster and the crushed spirit of productivity.}
  
    {~You pocket them — mission accomplished.|You slide the keys into your pocket like a responsible adult impersonator.|You pocket the keys and feel like someone who might just hold it all together today.|You secure the keys with the confidence of someone who’s definitely going to forget something else.|You pocket them and briefly consider rewarding yourself with a nap.|You drop them in your pocket, proud of yourself for winning the lowest-stakes scavenger hunt imaginable.}
 
    ~ inventory += car_keys
    -> go_to_frontroom

* [Leave for work]
    {~You glance at the door. It’s time. Ready or not.|You stare at the door like it’s a boss fight and you forgot to grind.|The door beckons. You answer, full of caffeine and misplaced optimism.|You look at the door, sigh like a protagonist, and prepare to face the day’s nonsense.|You approach the door like it owes you money and emotional stability.|You glance at the door, already tired of everything that might be waiting on the other side.|The door stands between you and responsibility. Unfortunately, you know how doors work.}
 
    -> leave_apartment
    
== leave_apartment ==

{~You step outside. The morning air hits you — not quite refreshing, but at least it's real. The world is waiting, indifferent and mildly damp.|You step out into the morning, where the sky is grey, the sidewalk's damp, and hope is optional.|You exit into a world that smells like wet concrete and subtle disappointment.|The air greets you like a passive-aggressive coworker: cool, damp, and vaguely judging.|You step outside, inhale, and immediately regret not checking the weather. Again.|The world welcomes you with chilly air, distant traffic, and the sound of someone else's better choices echoing down the block.}


* [Go for a walk]
    {~You start walking to work, convincing yourself it's for your health and not because gas is $6 a gallon.|You walk to work like it’s a lifestyle choice, not a financial negotiation with the universe.|You put one foot in front of the other and tell yourself it builds character — and saves $60 a week.|You walk with purpose, dignity, and the subtle rage of someone who saw the price at the pump.|You tell yourself walking is grounding, healthy, and not just because your car has trust issues.|You start walking, pretending it’s intentional and not because your budget screamed “NOPE.”}
 
    {~Birds chirp, your shoes complain, and you're already second-guessing this decision.|Birds sing cheerfully, oblivious to your growing blister and mild existential crisis.|Your shoes squish, a crow stares, and you're not sure this was the power move you imagined.|The birds are mocking you, your feet are plotting revenge, and it’s only been two blocks.|Nature greets you with chirping birds and the slow realization that this walk was a terrible idea.|Birds chirp like everything's fine, but your ankles and life choices strongly disagree.}
 
    ~ score += 1
    -> arrive_at_work

* [Take the Camry]
    {~You unlock the Camry and begin the sacred commute, surrounded by coffee cups and resignation.|You slide into the Camry, fire it up, and aim it toward work like a knight charging into mild paperwork.|The Camry starts without complaint — the only thing today that will.|You drive to work in the Camry: dependable, uninspired, and somehow still faster than your career.|The Camry rolls forward, steady as ever, taking you one pothole closer to professional mediocrity.|You steer the Camry toward work, where dreams go to schedule meetings and slowly dissolve.} 
    {~The AC smells like history, but it gets the job done.|The Camry hums with the quiet confidence of a car that’s outlived your last three relationships.|The fabric seats hold secrets. And probably crumbs. Mostly crumbs.|The glovebox hasn’t opened since 2018 and you’re too afraid to find out why.|One of the vents points directly at your face no matter what you do. You’ve accepted this.|The check engine light flickers occasionally, like it’s trying to warn you... about life.|The Camry makes a sound when you hit 40, but no one, not even mechanics, knows why.|There’s still a parking pass from a job you quit two years ago. You leave it. For the vibes.}
 
    ~ score += 1
    -> arrive_at_work

* [Take the T100 pickup truck]
    You climb into the T100. It starts with a roar and a wheeze, like an old warrior ready for one more battle.  
    The gas gauge mocks you, but the truck doesn’t care.  
    ~ score += 1
    -> arrive_at_work
    



== arrive_at_work ==
->fake_end