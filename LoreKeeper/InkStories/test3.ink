== main ==
You stand before a locked wooden door.

* [Try the handle] 
    The door is locked.
    -> DONE
* [Use the silver key]
    (You hold the key in your hand.)

-> DONE

== function use_item(itemName) ==
{itemName == "silver_key":
    ~ return "The key turns smoothly in the lock."
- else:
    ~ return "You try using {itemName}, but nothing happens."
}
