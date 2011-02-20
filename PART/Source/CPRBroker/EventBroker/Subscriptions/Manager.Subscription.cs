﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Engine;

namespace CprBroker.EventBroker.Subscriptions
{
    public static partial class Manager
    {
        /// <summary>
        /// This part contains methods related to admin interface
        /// All methods here simply delegate the code to Manager.CallMethod&lt;&gt;()
        /// </summary>
        public class Subscriptions
        {
            public static TOutput CallMethod<TInterface, TOutput>(string userToken, string appToken, bool allowLocalProvider, Func<TInterface, TOutput> func, bool failOnDefaultOutput, Action<TOutput> updateMethod) where TInterface : class, IDataProvider
            {
                return CprBroker.Engine.Manager.CallMethod<TInterface, TOutput>(userToken, appToken, allowLocalProvider, func, failOnDefaultOutput, updateMethod);
            }

            public static TOutput GetMethodOutput<TOutput>(FacadeMethodInfo<TOutput> facade) where TOutput : class, IBasicOutput, new()
            {
                return CprBroker.Engine.Manager.GetMethodOutput<TOutput>(facade);
            }

            #region Subscription
            public static BasicOutputType<ChangeSubscriptionType> Subscribe(string userToken, string appToken, ChannelBaseType notificationChannel, Guid[] personUuids)
            {
                SubscribeFacadeMethod facade = new SubscribeFacadeMethod(notificationChannel, personUuids, appToken, userToken);
                return GetMethodOutput<BasicOutputType<ChangeSubscriptionType>>(facade);

            }

            public static BasicOutputType<bool> Unsubscribe(string userToken, string appToken, Guid subscriptionId)
            {
                UnsubscribeFacadeMethod facade = new UnsubscribeFacadeMethod(subscriptionId, CprBroker.EventBroker.DAL.SubscriptionType.SubscriptionTypes.DataChange, appToken, userToken);
                return GetMethodOutput<BasicOutputType<bool>>(facade);
            }

            public static BasicOutputType<BirthdateSubscriptionType> SubscribeOnBirthdate(string userToken, string appToken, ChannelBaseType notificationChannel, Nullable<int> years, int priorDays, Guid[] PersonCivilRegistrationIdentifiers)
            {
                SubscribeOnBirthdateFacadeMethod facade = new SubscribeOnBirthdateFacadeMethod(notificationChannel, years, priorDays, PersonCivilRegistrationIdentifiers, appToken, userToken);
                return GetMethodOutput<BasicOutputType<BirthdateSubscriptionType>>(facade);
            }

            public static BasicOutputType<bool> RemoveBirthDateSubscription(string userToken, string appToken, Guid subscriptionId)
            {
                UnsubscribeFacadeMethod facade = new UnsubscribeFacadeMethod(subscriptionId, CprBroker.EventBroker.DAL.SubscriptionType.SubscriptionTypes.Birthdate, appToken, userToken);
                return GetMethodOutput<BasicOutputType<bool>>(facade);
            }

            public static BasicOutputType<SubscriptionType[]> GetActiveSubscriptionsList(string userToken, string appToken)
            {
                GetActiveSubscriptionsListFacadeMethod facade = new GetActiveSubscriptionsListFacadeMethod(appToken, userToken);
                return GetMethodOutput<BasicOutputType<SubscriptionType[]>>(facade);
            }

            #endregion

        }
    }
}
