// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Delay
{
    public class TwitterUser : INotifyPropertyChanged
    {
        public string ScreenName { get; private set; }
        public Uri ProfileImageUrl { get; private set; }
        public IEnumerable<TwitterUser> Followers
        {
            get
            {
                if (null == _followers)
                {
                    _followers = TwitterService.GetFollowers(ScreenName, () => FollowersLoaded = true);
                }
                return _followers;
            }
        }
        private IEnumerable<TwitterUser> _followers;
        public bool FollowersLoaded
        {
            get { return _followersLoaded; }
            set
            {
                _followersLoaded = value;
                InvokePropertyChanged("FollowersLoaded");
            }
        }
        private bool _followersLoaded;

        public TwitterUser(string screenName)
        {
            ScreenName = screenName;
        }
        public TwitterUser(string screenName, Uri profileImageUrl)
            : this(screenName)
        {
            ProfileImageUrl = profileImageUrl;
        }

        private void InvokePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
