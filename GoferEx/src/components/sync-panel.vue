<template>
  <!--<div v-for="auth in authSchemes" class="sign-btn" @click="signIn(auth.LoginAuthUri)">Sign in With {{auth.DisplayName}}</div>-->
  <ul id="example-1">
    <li v-for="auth in authSchemes" class="sign-btn" @click="signIn(auth)">Sign in With {{auth.DisplayName}}</li>
  </ul>
  <!---->
</template>
<script>
  import Vue from 'vue'
  import { mapMutations, mapState } from 'vuex'

  export default {
    computed: {
      ...mapState(['authSchemes'])
    },
    methods: {
      ...mapMutations(['syncUser']),
      signIn: function (authInfo) {
        let lu2 = 'https://localhost:1906/login' + authInfo.LoginAuthUri
        let authPop = window.open(lu2, authInfo.DisplayName);
        authPop.onbeforeunload = function (wind) {
          alert(wind)
        }
        alert('ssssss')
      }
    },
    created: function () {
      this.syncUser()     
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
