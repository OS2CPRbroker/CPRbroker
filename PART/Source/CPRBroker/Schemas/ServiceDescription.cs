﻿namespace CPRBroker.Schemas
{
    /// <summary>
    /// Contains text descriptions for all web services
    /// </summary>
    public class ServiceDescription
    {
        public abstract class Person
        {
            public const string Service = @"
                                                Contains methods related to persons' data
                                                ";

            #region CPR Person WS
            public const string GetCitizenNameAndAddress = @"
                                                Reads Name and address for a citizen used on a letter. 

                                                <br><br><b><u>Signature:</u></b>
                                                <br>PersonNameAndAddressStructureType GetCitizenNameAndAddress(string PersonCivilRegistrationIdentifier)

                                                <br><br><b><u>Parameter description:</u></b>
                                                <br><table>
                                                <tr><td>PersonCivilRegistrationIdentifier (input):</td><td>Person number to search for.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return PersonNameAndAddressStructure type that represent the standard XSD schema for name and address

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string GetCitizenBasic = @"
                                                Reads basic information about the citizen e.g. Name, Address,Civil status,Address protection, Municipality,
                                                Citizenship, Nationality, etc.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>PersonBasicStructureType GetCitizenBasic(string PersonCivilRegistrationIdentifier)

                                                <br><br><b><u>Parameter description:</u></b>
                                                <br><table>
                                                <tr><td>PersonCivilRegistrationIdentifier (input):</td><td>Person number to search for.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return PersonBasicStructure type that represent the standard XSD schema for basic information.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string GetCitizenFull = @"
                                                Reads all information about a particular citizen: name, address, school, election, church etc.,
                                                number of children. Other relevant information.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>PersonFullStructureType GetCitizenFull(string PersonCivilRegistrationIdentifier)

                                                <br><br><b><u>Parameter description:</u></b>
                                                <br><table>
                                                <tr><td>PersonCivilRegistrationIdentifier (input):</td><td>Person number to search for.</td></tr>
                                                </table>
                                                    
                                                <br><b><u>Return Value:</u></b>
                                                <br>return PersonFullStructure type that represent the standard XSD schema for the full information of person

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string GetCitizenRelations = @"
                                                Reads a particular citizens relations to parents, spouse, children and guardian etc.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>PersonRelationsType GetCitizenRelations(string PersonCivilRegistrationIdentifier)

                                                <br><br><b><u>Parameter description:</u></b>
                                                <br><table>
                                                <tr><td>PersonCivilRegistrationIdentifier (input):</td><td>Person number whose relationships are requested.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return PersonRelationsType that represent the standard XSD schema for person relations

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string GetCitizenChildren = @"
                                                Reads a particular citizens relations to own children and takes also reads children in custody
                                                using an argument.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>SimpleCPRPersonType[] GetCitizenChildren(string PersonCivilRegistrationIdentifier)

                                                <br><br><b><u>Parameter description:</u></b>
                                                <br><table>
                                                <tr><td>PersonCivilRegistrationIdentifier (input):</td><td>Person number to search for.</td></tr>
                                                <tr><td>IncludeCustodies (input):</td><td>Specifies whether to include custodied children or not.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return array of SimpleCPRPersonType.

                                                <br><br><b><u>Notes:</u></b>
                                                <br>Child age is not considered

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string RemoveParentAuthorityOverChild = @"
                                                Writes a change that removes a citizens custody over a child.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool RemoveParentAuthorityOverChild(string ParentCivilRegistrationIdentifier, string ChildCivilRegistrationIdentifier)

                                                <br><br><b><u>Parameter description:</u></b>
                                                <br><table>
                                                <tr><td>'cprNumber' (input):</td><td>Parent registeration number.</td></tr>
                                                <tr><td>'cprChildNumber' (input):</td><td>Child registeration number.</td></tr>
                                                </table>
                                                
                                                <br><b><u>Return Value:</u></b>
                                                <br>return bool that represents whether the operation has succeeded.

                                                <br><br><b><u>Notes:</u></b>
                                                <br>This method is independent from the person's chhildren. Custodies are considered a different type of relationship

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string SetParentAuthorityOverChild = @"
                                                Writes a citizen's custody over a child.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool SetParentAuthorityOverChild(string ParentCivilRegistrationIdentifier, string ChildCivilRegistrationIdentifier, string authorityCode, string custodyAuthorityCode, bool uncertainityMarker)

                                                <br><br><b><u>Parameter description:</u></b>
                                                <br><table>
                                                <tr><td>ParentCivilRegistrationIdentifier (input):</td><td>Parent registeration number.</td></tr>
                                                <tr><td>ChildCivilRegistrationIdentifier (input):</td><td>Child registeration number.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return bool that represents whether the operation has succeeded.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string GetParentAuthorityOverChildChanges = @"
                                                Reads a list of changes to custody changes done by this component.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>ParentAuthorityRelationshipType[] GetParentAuthorityOverChildChanges(string ChildCivilRegistrationIdentifier)

                                                <br><br><b><u>Parameter description:</u></b>
                                                <br><table>
                                                <tr><td>ChildCivilRegistrationIdentifier (input):</td><td>Child registeration number.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return an array of ParentAuthorityRelationshipType that represents the history of custodies for the specified child

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            #endregion
        }

        public abstract class Administrator
        {
            public const string Service = @"
                                                Contains methods related to administrative functions
                                                ";
            #region CPR Administration WS
            public const string RequestAppRegistration = @"
                                                Creates a new un-approved application in the system.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>ApplicationType RequestAppRegisteration(string ApplicationName)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>ApplicationName (input):</td><td>Application name that ask for registering.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return ApplicationType that specify the new appication.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string ApproveAppRegistration = @"
                                                Approves an application's registeration
                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool ApproveAppRegisteration(string ApplicationToken)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>'ApplicationToken' (input):</td><td>Token for the business client application to be approved.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return bool that represents whether the operation has succeeded.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string ListAppRegistrations = @"
                                                List Application Registeration
                                                <br><br><b><u>Signature:</u></b>
                                                <br>ApplicationType[] ListAppRegisteration()

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>Array of ApplicationType that represents the existing application registered in the system

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string UnregisterApp = @"
                                                Unregister an application registeration
                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool UnregisterApp(string ApplicationToken)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>ApplicationToken (input):</td><td>Security Token for the current business client application.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return bool that represents whether the operation has succeeded.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string GetCapabilities = @"
                                                Reads version information for the component to make sure that only features
                                                supported by the component is returned
                                                <br><br><b><u>Signature:</u></b>
                                                <br>string GetCapabilities(string userToken, string appToken)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>'userToken' (input):</td><td>Security Token for authorizing the request.</td></tr>
                                                <tr><td>'appToken' (input):</td><td>Security Token for the current business client application.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return the result of the request.

                                                <br><br><b><u>Notes:</u></b>
                                                <br> P1-P2 It is important that we use any standard or 'best practice' on this area. The sigature for
                                                this service is important. Syntax and semantic must be invariant over time. Response,
                                                probably in the form of an appropriate XML document will varie.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-05-11, CPR Broker</td><td width='10%'></td><td width='60%'>Initial Method Draft.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string IsImplementing = @"
                                                Direct (easier) alternative to GetCapabilities()

                                                <br><br><b><u>Signature:</u></b>
                                                <br>string IsImplementing(string userToken, string appToken,string serviceName,string serviceVersion)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>'userToken' (input):</td><td>Security Token for authorizing the user request.</td></tr>
                                                <tr><td>'appToken' (input):</td><td>Security Token for the current business client application.</td></tr>
                                                <tr><td>'serviceName' (input):</td><td>Service Name</td></tr>
                                                <tr><td>'serviceVersion' (input):</td><td>Service Version</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return the result of the request.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-05-11, CPR Broker</td><td width='10%'></td><td width='60%'>Initial Method Draft.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string GetDataProviderList = @"
                                                Returns a list of objects that contain information about the data providers that are currently used by the system.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>DataProviderType[] GetDataProviderList()

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>Array of DataProviderType containing information about the data providers used by the system.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string SetDataProviderList = @"
                                                Gives the opportunity to attach one or more data providers,
                                                e.g., CSC DPR, P-Data, online.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool SetDataProviderList(DataProviderType[] dataProviders)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>DataProviders (input):</td><td>Array of data providers that the system should use.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return bool that represents whether the operation has succeeded.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string Subscribe = @"
                                                Allows a client business application to be notified when there is a change in one or more person's data

                                                <br><br><b><u>Signature:</u></b>
                                                <br>ChangeSubscriptionType Subscribe(ChannelBaseType NotificationChannel, string[] PersonCivilRegistrationIdentifiers)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>NotificationChannel (input):</td><td>Channel to send notification through it (Web service, GPAC, FileShare...) .</td></tr>
                                                <tr><td>PersonCivilRegistrationIdentifiers[] (input):</td><td>Array of persons number that you want to subscribe the event to them. Null for all persons</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>ChangeSubscriptionType that represents the newly created subscription object

                                                <br><br><b><u>Notes:</u></b>
                                                <br> The component will keep track on any person that has already has been queried
                                                once. If the user creates a subscription, it will only be changes to the selected list of
                                                persons the  subscription will return. 

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string Unsubscribe = @"
                                                Removes a subscription.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool Unsubscribe(Guid SubscriptionId)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>SubscriptionId (input):</td><td>Subscription Id that you want to remove its subscription.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return bool that represents whether the operation has succeeded.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string SubscribeOnBirthdate = @"
                                                The user (application) can subscribe to extended birthday events.
                                                In case the business application needs to send a message to a citizen 3 weeks before
                                                the 65th. birthday (retirement), the user can subscribe to this event 65 birthday
                                                minus 3 weeks. This subscription can be created for all persons or for a specific list of persons. 
                                                It is possible to create several subscriptions
                                                from the same user application to the same person or list of indiviuals.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>BirthdateSubscriptionType SubscribeOnBirthdate(ChannelBaseType NotificationChannel, Nullable<int> Years, int PriorDays, string[] PersonCivilRegistrationIdentifiers)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>NotificationChannel (input):</td><td>Channel to send notification through it (Web service, GPAC, FileShare...) .</td></tr>
                                                <tr><td>Years (input):</td><td>Years.</td></tr>
                                                <tr><td>PriorDays (input):</td><td>Prior days.</td></tr>
                                                <tr><td>PersonCivilRegistrationIdentifiers[] (input):</td><td>Array of persons number that you want to subscribe the event to them.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>BirthdateSubscriptionType that represents the created subscription object.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string RemoveBirthDateSubscriptions = @"
                                                Removes one extended subscription for a user application.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool RemoveBirthDateSubscription(Guid SubscriptionId)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>'SubscriptionId' (input):</td><td>Subscription Id that you want to remove its subscription.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return bool that represents whether the operation has succeeded.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string GetActiveSubscriptionsList = @"
                                                Allows a business application to get a list of all subscriptions

                                                <br><br><b><u>Signature:</u></b>
                                                <br>SubscriptionType[] GetActiveSubsciptionsList()

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>Array of SubscriptionType that represents the current subscriptions

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string GetLatestNotification = @"
                                                Allows a business application to get the latest notification that has been fired for a given subscription
                                                <br>Usually called as a callback after a notification event is fired

                                                <br><br><b><u>Signature:</u></b>
                                                <br>SubscriptionType[] GetLatestNotification()

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>'SubscriptionId' (input):</td><td>Subscription Id for which the latest notification is required.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>BaseNotificationType that represents the latest notification fired for the subscription

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string Log = @"
                                                Writes a text string to the system's log
                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool Log(string Text)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>Text (input):</td><td>the text value to be logged.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return bool that represents whether the operation has succeeded.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string CreateTestCitizen = @"
                                                Creates a non-existing  citizen (Dummy), so that the services can later return a usable query result. 

                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool CreateTestCitizen(PersonFullStructureType OioPerson)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>OioPerson (input):</td><td>PersonFullStructureType that represent the user you want to create as a test citizen</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return bool that represents whether the operation has succeeded.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            #endregion
        }

        public static class Access
        {
            public const string Service = "Allows web access for certain administrative functions";

            public const string SendNotifications = @"            
                                                Causes the system to check for due notifications and send them

                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool SendNotifications(DateTime today)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>today (input):</td><td>Due date of notifications</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return SendNotificationsResult that represents the result of the operation.
                                                
                                                <br>==============================
                                                ";

            public const string RefreshPersonsData = @"            
                                                Causes the system to refresh data of all the persons that are part of a data change subscription

                                                <br><br><b><u>Signature:</u></b>
                                                <br>RefreshPersonsDataResult RefreshPersonsData()

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>                                                
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return RefreshPersonsDataResult that represents the result of the operation.
                                                
                                                <br>==============================
                                                ";

        }

        public static class Notification
        {
            public const string Service = "Template for a notification listener web service";

            public const string Notify = @"            
                                                Called to tell the client system that a notification has been fired

                                                <br><br><b><u>Signature:</u></b>
                                                <br>void Notify(string appToken, BaseNotificationType notification)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>appToken (input):</td><td>Token of subscriber application</td></tr>
                                                <tr><td>notification (input):</td><td>Object that contains the detailed information that is associated with the event</td></tr>
                                                </table>

                                                <br>==============================
                                                ";

            public const string Ping = @"            
                                                Called to make sure the service is online

                                                <br><br><b><u>Signature:</u></b>
                                                <br>void Ping()

                                                <br>==============================
                                                ";


        }
    }
}