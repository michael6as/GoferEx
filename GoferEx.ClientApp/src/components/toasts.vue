<template>
  <div class="toasts-wrapper">
      <div class="toast" v-for="toast in toasts" :key="toast.id">
          <img class="toast-icon" :src="getIcon(toast)"/>
          <span>{{toast.message}}</span>
          <img class="remove-icon" :src="closeIcon" @click="removeToast(toast)"/>
      </div>
  </div>
</template>

<script>
import {mapState,mapMutations} from 'vuex'
import VIcon from '@/assets/v-icon.svg'
import XIcon from '@/assets/x-icon.svg'
import closeIcon from '@/assets/remove-icon.svg'

export default {
    data (){
        return {
            closeIcon
        }
    },
    computed:{
        ...mapState(['toasts'])
    },
    methods:{
        ...mapMutations(['removeToast']),
        getIcon(toast){
            if(toast.isPositive) return VIcon
            return XIcon
        }
    }
}
</script>

<style lang="scss" scoped>
.toasts-wrapper{
    position: absolute;
    right:300px;
    top: 30px;
    display: flex;
    flex-direction: column;
    justify-content: flex-end;

    .toast{
        height: 36px;
        width: fit-content;
        border-radius: 4px;
        background-color: #ffffff;
        box-shadow: 0 2px 10px 0 rgba(0, 0, 0, 0.08);
        color: #535a5f;
        font-size: 13px;
        display: flex;
        align-items: center;
        padding: 0 14px;
        margin-left: auto;
        margin-bottom: 20px;
        

        .toast-icon{
            margin-right: 8px;
        }

        span{
            margin-right: 20px;
        }
    }
}
</style>
