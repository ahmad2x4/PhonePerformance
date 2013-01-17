// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Xml.Linq;

namespace Delay
{
    public static class TwitterService
    {
        public static IEnumerable<TwitterUser> GetFollowers(string screenName)
        {
            return GetFollowers(screenName, null);
        }
        public static IEnumerable<TwitterUser> GetFollowers(string screenName, Action onFollowersLoaded)
        {
            var collection = new ObservableCollection<TwitterUser>();
            MakeGetFollowersRequest(screenName, "-1", collection, onFollowersLoaded);
            return collection;
        }

        private static void MakeGetFollowersRequest(string screenName, string cursor, ObservableCollection<TwitterUser> collection, Action onFollowersLoaded)
        {
            var request = HttpWebRequest.CreateHttp("http://api.twitter.com/1/statuses/followers.xml?screen_name=" + screenName + "&cursor=" + cursor);
            var state = new GetFollowersState(screenName, collection, request, onFollowersLoaded);
            request.BeginGetResponse(HandleGetFollowersResponse, state);
        }

        private static void HandleGetFollowersResponse(IAsyncResult result)
        {
            var state = (GetFollowersState)result.AsyncState;
#if DEBUG
            try
            {
#endif
                using (var response = state.Request.EndGetResponse(result))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var document = XDocument.Load(stream);
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var user in document.Element("users_list").Element("users").Elements("user"))
                            {
                                state.Collection.Add(new TwitterUser(user.Element("screen_name").Value, new Uri(user.Element("profile_image_url").Value)));
                            }
                        });
                        var nextCursor = document.Element("users_list").Element("next_cursor").Value;
                        if ("0" == nextCursor)
                        {
                            // Load completed
                            if (null != state.OnFollowersLoaded)
                            {
                                Deployment.Current.Dispatcher.BeginInvoke(() => state.OnFollowersLoaded());
                            }
                        }
                        else
                        {
                            // Load the next set
                            MakeGetFollowersRequest(state.ScreenName, nextCursor, state.Collection, state.OnFollowersLoaded);
                        }
                    }
                }
#if DEBUG
            }
            catch (WebException)
            {
                // No network access; create some fake users for debugging purposes
                for (var i = 0; i < 200; i++)
                {
                    state.Collection.Add(new TwitterUser("Fake User " + i));
                }
                if (null != state.OnFollowersLoaded)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => state.OnFollowersLoaded());
                }
            }
#endif
        }

        private class GetFollowersState
        {
            public string ScreenName { get; private set; }
            public ObservableCollection<TwitterUser> Collection { get; private set; }
            public HttpWebRequest Request { get; private set; }
            public Action OnFollowersLoaded { get; private set; }
            public GetFollowersState(string screenName, ObservableCollection<TwitterUser> collection, HttpWebRequest request, Action onFollowersLoaded)
            {
                ScreenName = screenName;
                Collection = collection;
                Request = request;
                OnFollowersLoaded = onFollowersLoaded;
            }
        }
    }
}
