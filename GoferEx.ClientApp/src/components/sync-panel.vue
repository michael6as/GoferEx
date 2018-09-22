<template>
  <div class="sync-panel-wrapper">
    <div class="header-md">Sync with external provider</div>
    <div class="sign-btn-wrapper">
      <div class="sign-btn" v-for="auth in authSchemes"><a :href="auth.LoginChallenge">{{auth.DisplayName}}</a></div>
    </div>
  </div>
</template>
<script>
  import Vue from 'vue'
  import { mapMutations, mapState } from 'vuex'

  export default {
    computed: {
      ...mapState(['authSchemes'])
    },
    methods: {
      ...mapMutations(['loginChallenge', 'syncContacts']),
      signIn: function (authInfo) {
        let lu2 = 'http://localhost:1995/login' + authInfo.LoginAuthUri
        let authPop = window.open(lu2, authInfo.DisplayName);
        authPop.onclose = function () {
          this.syncContacts()
        }
      }
    },
    created: function () {
      this.loginChallenge()
    }
  }
</script>

<style>
  .sync-panel-wrapper {
    background-color: #f5f7fa;
    padding: 1%;
  }

  .sign-btn-wrapper {
    padding-top: 6%;
    flex: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
  }

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

  .signed-btn {
    background-color: aquamarine;
  }
</style>
