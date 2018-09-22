<template>
<div class="users-list-wrapper">
    <div class="header header-md"> Gofer Users </div>
    <user-list-item v-for="user in users" :user="user" :key="user.id" @click.native="selectUser(user)"/>
</div>
</template>

<script>
import UserListItem from './user-list-item'
import {mapMutations, mapState} from 'vuex'

export default {
  components: {UserListItem},
  computed: {
    ...mapState(['users'])
  },
  methods: {
    ...mapMutations(['getContacts', 'selectUser'])
  },
  created: function () {
    this.getContacts()
  }
}
</script>

<style lang="scss" scoped>
.users-list-wrapper{
    flex:0 0 280px;
    box-shadow: 0 0 8px 0 rgba(0, 0, 0, 0.09);
    color: #131b3c;

    .header{
        height:64px;
        border-bottom: solid 1px #e6e9f2;
        padding-left: 20px;
        display: flex;
        align-items: center;
    }
}
</style>
