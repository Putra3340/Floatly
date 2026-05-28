using CommunityToolkit.Mvvm.Messaging.Messages;
using Floatly.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Floatly.Messages
{
    // Message
    // this is now not used, but i will keep it for later use
    public class SongChangedMessage(Song song) : ValueChangedMessage<Song>(song);
}
