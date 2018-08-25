import _ from 'lodash'
import uuid from 'uuid'
import Vue from 'vue'
import Vuex from 'vuex'
import defaultUsers from './default-users'

Vue.use(Vuex)

const localStorage = window.localStorage

function updateStorage (users) {
  localStorage.setItem('users', JSON.stringify(users))
}

var existUsers = localStorage.getItem('users') ? JSON.parse(localStorage.getItem('users')) : defaultUsers

export default new Vuex.Store({
  state: {
    users: existUsers,
    toasts: [],
    currentUser: null
  },
  mutations: {
    addUser (state, user) {
      let users = state.users

      if (!user.id) {
        user.id = uuid()
      }

      let idx = _.findIndex(users, {id: user.id})
      let toast
      if (idx !== -1) {
        users.splice(idx, 1, user)
        toast = {id: uuid(), message: 'User updated', isPositive: true}
      } else {
        users.push(user)
        toast = {id: uuid(), message: 'User added', isPositive: true}
      }

      state.users = users
      state.toasts.push(toast)
      setTimeout(() => {
        state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
      }, 3000)

      updateStorage(state.users)
    },
    removeUser (state, user) {
      state.users = _.remove(state.users, (u) => u.id !== user.id)
      if (state.currentUser.id === user.id) {
        state.currentUser = null
      }

      let toast = {id: uuid(), message: 'User removed', isPositive: false}
      state.toasts.push(toast)
      setTimeout(() => {
        state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
      }, 3000)

      updateStorage(state.users)
    },
    selectUser (state, user) {
      state.currentUser = user
    },
    removeToast (state, toast) {
      state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
    }

  }
})
