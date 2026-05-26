using CommunityToolkit.Mvvm.Messaging.Messages;
using Floatly.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Floatly.Messages
{
    // Message
    public class SongChangedMessage(Song song) : ValueChangedMessage<Song>(song);
}
