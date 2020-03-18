// Copyright © 2020 Shawn Baker using the MIT License.
using System;
using System.ComponentModel;

namespace FrozenNorth.SpotifyAuth
{
    [Flags]
    public enum Scope
    {
        None = 0,

        [Description("ugc-image-upload")]
        UgcImageUpload = 0x00001,

        [Description("playlist-read-private")]
        PlaylistReadPrivate = 0x00002,

        [Description("playlist-modify-public")]
        PlaylistModifyPublic = 0x00004,

        [Description("playlist-read-collaborative")]
        PlaylistReadCollaborative = 0x00008,

        [Description("playlist-modify-private")]
        PlaylistModifyPrivate = 0x00010,

        [Description("user-read-currently-playing")]
        UserReadCurrentlyPlaying = 0x00020,

        [Description("user-modify-playback-state")]
        UserModifyPlaybackState = 0x00040,

        [Description("user-read-playback-state")]
        UserReadPlaybackState = 0x00080,

        [Description("user-read-recently-played")]
        UserReadRecentlyPlayed = 0x0100,

        [Description("user-top-read")]
        UserTopRead = 0x00200,

        [Description("user-follow-read")]
        UserFollowRead = 0x00400,

        [Description("user-follow-modify")]
        UserFollowModify = 0x00800,

        [Description("app-remote-control")]
        AppRemoteControl = 0x01000,

        [Description("streaming")]
        Streaming = 0x02000,

        [Description("user-read-private")]
        UserReadPrivate = 0x04000,

        [Description("user-read-email")]
        UserReadEmail = 0x08000,

        [Description("user-library-modify")]
        UserLibraryModify = 0x10000,

        [Description("user-library-read")]
        UserLibraryRead = 0x20000,
    }
}
