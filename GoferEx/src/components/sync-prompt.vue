<template>
  <g-signin-button :params="googleSignInParams"
                   @success="onSignInSuccess"
                   @error="onSignInError">
    Sign in with Google
  </g-signin-button>
</template>

<script>
  import { mapMutations, mapState } from 'vuex'

  export default {
    data() {
      return {
        /**
         * The Auth2 parameters, as seen on
         * https://developers.google.com/identity/sign-in/web/reference#gapiauth2initparams.
         * As the very least, a valid client_id must present.
         * @type {Object}
         */
        googleSignInParams: {
          client_id: '535817455358-r5dc38s94p747i89irsu649mkh75dk78.apps.googleusercontent.com'
        }
      }
    },
    methods: {
      ...mapMutations(['syncUser']),
      onSignInSuccess(googleUser) {
        // `googleUser` is the GoogleUser object that represents the just-signed-in user.
        // See https://developers.google.com/identity/sign-in/web/reference#users
        console.log(googleUser.getBasicProfile())
        console.log(googleUser)
        this.syncUser(googleUser)
        const profile = googleUser.getBasicProfile() // etc etc
      },
      onSignInError(error) {
        // `error` contains any error occurred.
        console.log('OH NOES', error)
      }
    }
  }
</script>

<style>
  .g-signin-button {
    /* This is where you control how the button looks. Be creative! */
    display: inline-block;
    padding: 4px 8px;
    border-radius: 3px;
    background-color: #3c82f7;
    color: #fff;
    box-shadow: 0 3px 0 #0f69ff;
  }
</style>
