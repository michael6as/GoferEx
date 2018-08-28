<template>
  <div class="sign-btn" @click="signIn">Sign In With Google</div>
</template>
<script>
  import Vue from 'vue'
  import { mapMutations } from 'vuex'

  export default {
    methods: {
      ...mapMutations(['syncUser']),
      signIn: function () {
        let loginUrl = 'https://accounts.google.com/o/oauth2/auth?redirect_uri=http://localhost:1906/signin-google&response_type=code&client_id=535817455358-tlgkg5jca5u3kjd3jlhqr4u1ef6udt7f.apps.googleusercontent.com&scope=https://www.googleapis.com/auth/userinfo.profile+https://www.googleapis.com/auth/plus.me+https://www.googleapis.com/auth/plus.profile.language.read+https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/plus.profile.agerange.read+https://www.googleapis.com/auth/contacts.readonly&approval_prompt=force&access_type=offline'
        window.open(loginUrl)
      }
    }
  }
</script>

<style>
  .sign-btn {
    margin-top: 0.5%;
    width: 130px;
    height: 30px;
    border-radius: 4px;
    font-size: 13px;
    line-height: 1;
    color: #fff;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: dodgerblue
  }
</style>
