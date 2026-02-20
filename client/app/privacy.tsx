import React from 'react';
import { ScrollView, Text, View } from 'react-native';
import useStyles from './styles';
import useAppTheme from './theme';

export const options = { headerShown: true };

export default function PrivacyPolicyScreen() {
  const styles = useStyles();
  const theme = useAppTheme();

  return (
    <ScrollView style={{ flex: 1, backgroundColor: theme.colors.background }}>

      <View style={{ padding: 20 }}>
        <View style={{
          borderRadius: 16,
          shadowColor: '#000',
          shadowOpacity: 0.05,
          shadowRadius: 8,
          shadowOffset: { width: 0, height: 2 }
        }}>
          <Text style={{ ...styles.tinyText, marginBottom: 12, lineHeight: 22 }}>
            We respect your privacy as the user and will not sell or share your personal data with any third parties. Your information and content are solely used to provide features and to improve the app.
            Below we list what data we collect and how it is used.
          </Text>
          <Text style={{ ...styles.tinyText, marginBottom: 12, lineHeight: 22 }}>
            Your username is stored to serve as your unique identifier within the app and is shown on your profile. Your username can be searched and seen publicly by anyone on the platform.
            We store your email as a way to validate your identify during logon attempts and for account recovery and security. Your email can only be seen by you and approved admins for contact purposes. Your email is end-to-end encrypted.
            Your first and last name are also collected and are publicly available to anyone on the platform. Anyone can search for you using your full name.
            Your password is used to verify your identify and to secure your account. It is hashed immediately upon account creation or updating your password, unable to be decrypted, and not available for anyone to see unencrypted, including you. During logon, your inputed password is hashed and compared to your stored password hash to verify your identify.
            Data relating to your activity is tracked, such as your auth token for authomatic logons, your activity status, account creation date, and last seen date are tracked. Activity status and last seen dates are visible by your friends on the platform.
            Data relating to your profile are tracked, such as profile description, profile picture, number of private/public collections, number of memorized passages, and points.
            Profile description is private to you and your friends, and number of collections and memorized passages is private to you. Your profile picture and points are public.
            Your account preferences are stored, such as those found on the settings page. All preferences are private to you.
            We store data relating to the paid features of the app, and may store last payment date, payment active, and subscriptione expired date, and more... [update this]
          </Text>
          <Text style={{ ...styles.tinyText, marginBottom: 12, lineHeight: 22 }}>
            Your user-created collections and everything they include (title, description, passages, notes) are stored and unencrypted. Whether they are private or public is determined by some factors.
            Unpublished collections and their passages and notes marked as "private" are only visible to you, not even administrators, but are stored unencrypted in the database. 
            Collections you choose to set as "public" are visible only to your friends when visiting your profile.
            Collections you choose to publish are made publicly visible to anyone on the platform and are potentially tied to your username, full name, and profile picture. They are first seen by content administrators, then approved and visible publicly.
            Updating a private collection, after a published collection has been created from it, does not update the published collection, and all changes are kept private unless you update the published collection.
            Updating a published collection automatically pushes all changes to all users who have saved that collection. Unpublishing a published collection will remove all traces of it, and users will not be able to see or find it.
            A collection marked as "private" is secure and private to you. Passages contained in a private collection are also private, and memorizing activity on one is also private. The only public data updated from this action would be your points increasing.
          </Text>
          <Text style={{ ...styles.tinyText, marginBottom: 12, lineHeight: 22 }}>
            When reading the Bible, some information is tracked to improve your interaction.
            While all passages you visit are not tracked, the last passage you visit before leaving the page or closing the app is tracked to keep your place, and is private to you.
            Bookmarks, highlights, and private notes are tracked and private to you.
            Private notes you create for a passage are end-to-end encrypted and only visible to you. If you choose to make a note public, it is visible publicly and potentially tied to your username and full name.
            Public notes you like are tracked and are not able to be tied back to your identity by normal users or administrators.
          </Text>
          <Text style={{ ...styles.tinyText, marginBottom: 12, lineHeight: 22 }}>
            Your activity on the app may be tracked as logs, analytics, or historical data for the purpose of bug tracking, performance monitoring, security, and improving user experience. 
            This information may include timestamps, the feature being used, and the action done.
          </Text>
          <Text style={{ ...styles.tinyText, marginBottom: 12, lineHeight: 22 }}>
            We do not track information relating to your device or location.
          </Text>
          <Text style={{ ...styles.tinyText, marginBottom: 12, lineHeight: 22 }}>
            At any time, you may request to have any data related to your account permanently deleted from our system by contacting us at: [email]
            When you request to be deleted from our system, any trace of your account and information will be deleted, with the exception of aggregate user activity that includes you, such as a count of users who have saved a verse.
            Data that will be deleted includes account info, private and public content tied to you, activity logs, analytics, and anything directly identifiable to you.
          </Text>
        </View>
          <Text style={{
            fontSize: 12,
            color: theme.colors.onBackground,
            opacity: 0.8,
            marginTop: 6,
            fontFamily: 'Inter'
          }}>
            Last updated: February 15, 2026
          </Text>
      </View>
      <View style={{ height: 60 }} />
    </ScrollView>
  );
}


