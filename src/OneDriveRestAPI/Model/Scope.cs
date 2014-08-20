using System.ComponentModel;

namespace OneDriveRestAPI.Model
{
    public enum Scope
    {
        [Description("wl.basic")] Basic,
        [Description("wl.offline_access")] OfflineAccess,
        [Description("wl.signin")] Signin,
        [Description("wl.basic")] Birthday,
        [Description("wl.calendars")] Calendars,
        [Description("wl.calendars_update")] CalendarsUpdate,
        [Description("wl.contacts_birthday")] ContactsBirthday,
        [Description("wl.contacts_create")] ContactsCreate,
        [Description("wl.contacts_calendars")] ContactsCalendars,
        [Description("wl.contacts_photos")] ContactsPhotos,
        [Description("wl.contacts_skydrive")] ContactsSkyDrive,
        [Description("wl.emails")] Emails,
        [Description("wl.events_create")] EventsCreate,
        [Description("wl.messenger")] Messenger,
        [Description("wl.phone_numbers")] PhoneNumbers,
        [Description("wl.photos")] Photos,
        [Description("wl.postal_addresses")] PostalAddress,
        [Description("wl.share")] Share,
        [Description("wl.skydrive")] SkyDrive,
        [Description("wl.skydrive_update")] SkyDriveUpdate,
        [Description("wl.work_profile")] WorkProfile,
        [Description("wl.applications")] Applications,
        [Description("wl.applications_create")] ApplicationsCreate
    }
}