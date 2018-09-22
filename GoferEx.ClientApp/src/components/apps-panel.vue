<template>
  <div class="apps-panel-wrapper">
      <div class="app-panel-item" v-for="(value,key) in apps" :key="key" :class="{selected:key === 'contacts'}">
          <img :src="value" :title="key"/>
      </div>
  </div>
</template>

<script>
const apps = {
  'all': require('@/assets/apps/all.svg'),
  'files': require('@/assets/apps/files.svg'),
  'messages': require('@/assets/apps/messages.svg'),
  'calander': require('@/assets/apps/calander.svg'),
  'todo': require('@/assets/apps/todo.svg'),
  'contacts': require('@/assets/apps/contacts.svg'),
  'desktop': require('@/assets/apps/desktop.svg'),
  'photos': require('@/assets/apps/photos.svg')
}
export default {
  data () {
    return {
      apps,
      currentApp: 'contacts'
    }
  }
}
</script>

<style lang="scss" scoped>
.apps-panel-wrapper{
  flex:0 0 70px;
  background-color: #454c68;
  box-shadow: 0 0 8px 0 rgba(0, 0, 0, 0.08);
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-top: 20px;
}

.app-panel-item{
    width:40px;
    height:40px;
    border-radius: 50%;
    background-color: rgba(255, 255, 255, 0.2);
    margin-bottom: 25px;
    display:flex;
    align-items: center;
    justify-content: center;

    &.selected{
        background-color: #47a0f9;
        border: solid 1px #459bf3;
    }

    //cuz images are bad
    img{
        margin-left: 4px;
        margin-top:5px;
    }
}
</style>
