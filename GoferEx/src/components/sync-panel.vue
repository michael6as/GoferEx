<template>
  <div class="container container-table">
    <!-- Errors -->
    <div v-if=response class="text-red"><p>{{response}}</p></div>

    <!-- login Button -->
    <a id="signin-button" v-on:click="signIn">
      <i class="fa fa-google-plus-official fa-3x"></i>
      Sign in with Google
    </a>
  </div>
</template>
<script>
  import Vue from 'vue'
  import { mapMutations, mapState } from 'vuex'

  export default {
    data(router) {
      return {
        section: 'Login',
        loading: '',
        response: ''
      }
    },
    methods: {
      ...mapMutations(['syncUser']),
      signIn: function () {
        let syncUser = this.syncUser

        // This lets me get the googleUser object rather than the auth code
        Vue.googleAuth().directAccess()
          Vue.googleAuth().signIn(this.onSignInSuccess, this.onSignInError)
      },
      onSignInSuccess: function (googleUser) {
        this.syncUser({ token: googleUser, provider: 'Google' })
        // This line is redirecting me to this url with the auth code and other things from Google
        googleUser.grantOfflineAccess({ 'redirect_uri': 'http://localhost:1906/signin-google' }).then(function (response) {
          syncUser({ token: response.code, provider: 'Google' })
          //this.toggleLoading()
          //this.resetResponse()
        }, function (error) {
          console.log(error)
        })      
        // this.syncUser({ token: authorizationCode, provider: 'Google' })
      },
      onSignInError: function (error) {
        this.response = 'Failed to sign-in'
        console.log('GOOGLE SERVER - SIGN-IN ERROR', error)
      },
      toggleLoading: function () {
        this.loading = (this.loading === '') ? 'loading' : ''
      },
      resetResponse: function () {
        this.response = ''
      }
    }

  }
</script>

<style>
  /**
  * ----------------------------------------------------
  *  Styling? It's time to show your designing skills!
  * ----------------------------------------------------
  */
</style>
