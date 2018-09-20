<template>
  <div class="user-list-item-wrapper">
      <img class="user-photo" :src="user.photo"/>
      <span class="user-name"> {{user.firstName}} {{user.lastName}} </span>
      <img class="delete-icon" :src="deleteIcon" @click.stop="removeUser(user)"/>

      <div class="user-tooltip-wrapper">

        <div class="user-tooltip-triangle">
          <div class="triangle"/>
          <div class="cover"/>
        </div>

        <div class="user-tooltip">
          <div class="photo-and-name">
            <img :src="user.photo"/>
            <span>{{user.username}}</span>
          </div>
          <div class="details">
              <span class="bold">{{user.firstName}} {{user.lastName}}</span>
              <span> {{user.email}}</span>
              <span> {{user.birthdate}} </span>
              <span> {{user.phone}} </span>
          </div>
        </div>

      </div>
  </div>
</template>

<script>
import deleteIcon from '@/assets/remove-icon.svg'
import {mapMutations} from 'vuex'

export default {
  props: ['user'],
  data () {
    return {
      deleteIcon
    }
  },
  methods:{
    ...mapMutations(['removeUser'])
  }
}
</script>

<style lang="scss" scoped>
.user-list-item-wrapper{
  position: relative;
  display: flex;
  align-items: center;
  padding:0 20px;
  height:64px;
  border-bottom: solid 1px #e6e9f2;

  &:hover{
    background-color: #f5f7fa;

    .delete-icon{
      display: initial;
    }

    .user-tooltip-wrapper{
      display: initial;
    }
  }

  .user-photo{
    width:30px;
    height:30px;
    border-radius: 50%;
  }

  .user-name{
    flex:1;
    margin-left:12px;
    text-overflow: ellipsis;
    overflow: hidden;
    white-space: nowrap;
  }

  .delete-icon{
    display:none;
  }
}

.user-tooltip-wrapper{
  position: absolute;
  display:none;
  top:64px;
  left:43px;
  z-index:2;
}

.user-tooltip-triangle{
  position: absolute;
  left:40px;
  z-index: 2;

  .triangle{
    width: 20px;
    height:20px;
    transform: rotate(45deg);
    background-color: white;
    box-shadow: 0 1px 6px 0 rgba(0, 0, 0, 0.1);
    border: solid 1px #e6e9f2;
  }

  .cover{
    position: absolute;
    top:11px;
    width:40px;
    height:20px;
    margin-left: -10px;
    background-color: #fff;
    z-index:2;
  }

}

.user-tooltip{
  position: absolute;
  top:10px;
  display:flex;
  width: 282px;
  height:135px;
  box-shadow: 0 1px 6px 0 rgba(0, 0, 0, 0.1);
  border: solid 1px #e6e9f2;
  background-color: #fff;
  font-size: 12px;

  .photo-and-name{
    width: 100px;
    border-right:  solid 1px #e6e9f2;
    padding: 20px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    align-items: center;

    img{
      width: 60px;
      height:60px;
      border-radius: 50%;
    }
  }

  .details{
    display: flex;
    flex-direction: column;
    padding:20px;

    .bold{
      font-weight: bold;
      color: #131b3c;
    }
    span{
      margin-bottom:10px; 
      color: #a5a8b5;
    }
  }
}
</style>
