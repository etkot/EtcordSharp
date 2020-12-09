type ClientState = 'Unconnected' | 'Handshaking' | 'Login' | 'Connected'

type Channel = {
    ChannelID: number
    ParentID: number
    Name: string
    Type: 'None' | 'TextChat' | 'VoiceChat' | 'Both'
}

type Message = {
    ChannelID: number
    MessageID: number
    SenderID: number
    SenderName: string
    Content: string
}

type User = {
    UserID: number
    Name: string
    IsLocal: bool
    VoiceChannelID: number
}

interface Events {
    ClientStateChanged: ClientState
    ChannelAdded: Channel
    ChannelUpdated: Channel
    MessageAdded: Message
    UserAdded: User
    UserJoinVoice: { User: User; Channel: Channel }
    UserLeaveVoice: { User: User; Channel: Channel }
    UserUpdated: User
}

interface EventEmitter {
    on<EventType extends keyof Events>(
        event: EventType,
        listener: (data: Events[EventType]) => void
    )
    removeListener<EventType extends keyof Events>(
        event: EventType,
        listener: (data: Events[EventType]) => void
    )
}

declare var events: EventEmitter

declare function etcord_Connect(
    address: string,
    username: string
): Promise<void>
