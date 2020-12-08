# Etcord api documentation
Here you will find the events that Etcord sends and the functions Etcord exposes to the frontend



## Listening for Events
The C# backend sends events to the frontend through the global 'events' object

```js
let listener = data => { /* Do something */ }

// Subscribe to an event with
events.on('event name', listener)

// Unsubscribe from an event with
events.removeListener('event name', listener)
```

Event data is always structured like this no matter how many fields they have
```js
// Example of an OnUserJoinVoice event data
{
    user: {
        UserID: 1,
        Name: 'Username',
        IsLocal: true,
        VoiceChannelID: 3
    },
    channel: {
        ChannelID: 3,
        ParentID: 0,
        Name: 'Games',
        Type: 'Both'
    }
}
```



### Structs

Events have data with them and this data is structured based on what kind of data it is
Data is a single object that has one or more structs inside

#### ClientState

String containing one of the following options
- Unconnected
- Handshaking
- Login
- Connected

#### Channel

Object that has the following fields

-----------------------------------------------------------------------------
| Name           | Type         | Notes                                     |
-----------------------------------------------------------------------------
| ChannelID      | int          |                                           |
| ParentID       | int          |                                           |
| Name           | string       |                                           |
| Type           | string       | 'None', 'TextChat', 'VoiceChat' or 'Both' |
-----------------------------------------------------------------------------

#### Message

Object that has the following fields

-----------------------------------------------------------------------------
| Name           | Type         | Notes                                     |
-----------------------------------------------------------------------------
| ChannelID      | int          |                                           |
| MessageID      | int          |                                           |
| SenderID       | int          |                                           |
| SenderName     | string       |                                           |
| Content        | string       |                                           |
-----------------------------------------------------------------------------

#### User

Object that has the following fields

-----------------------------------------------------------------------------
| Name           | Type         | Notes                                     |
-----------------------------------------------------------------------------
| UserID         | int          |                                           |
| Name           | string       |                                           |
| IsLocal        | bool         |                                           |
| VoiceChannelID | int          |                                           |
-----------------------------------------------------------------------------



### Events

----------------------------------------------
| Name               | Data                  |
----------------------------------------------
| ClientStateChanged | ClientState           |
| ChannelAdded       | Channel               |
| ChannelUpdated     | Channel               |
| MessageAdded       | Message               |
| UserAdded          | User                  |
| UserJoinVoice      | User, Channel         |
| UserLeaveVoice     | User, Channel         |
| UserUpdated        | User                  |
----------------------------------------------



## Functions
Etcord exposes some functions so that it's possible to control Etcord from the frontend
All functions will return a promise

### etcord_Connect(string address, string username)
Address can have a port specified but it's not required
If there is no port specified Etcord will user the default port 3879
Username cannot be empty
Error checking doesn't have to be done on the frontend

If address or username is invalid an exeption will be thrown
Otherwise the promise will be resolved
ClientStateChanged event will be fired to inform about the connection
