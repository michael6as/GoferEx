import _ from 'lodash'
import uuid from 'uuid'
import Vue from 'vue'
import Vuex from 'vuex'
import defaultUsers from './default-users'
import axios from 'axios'

Vue.use(Vuex)

const myApi = axios.create({
  baseURL: 'http://localhost:1906/api/',
  withCredentials: true,
  headers: {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  }
})

// TODO: Change here to basic GET
const localStorage = window.localStorage

// TODO: Change here to basic POST
function updateStorage (users) {
  localStorage.setItem('users', JSON.stringify(users))
}

// TODO: Chnage based on line 10
myApi.get('contact').then(res => {
  console.log('Update storage func2')
  localStorage.setItem('users', JSON.stringify(res.data))
}).catch(e => {
  alert(e)
})

var existUsers = localStorage.getItem('users') ? JSON.parse(localStorage.getItem('users')) : defaultUsers

export default new Vuex.Store({
  state: {
    users: existUsers,
    toasts: [],
    currentUser: null,
    syncedTokens: []
  },
  mutations: {
    addUser (state, user) {
      if (!user.id) {
        user.id = uuid()
      }
      myApi.post('contact', user).then(res => {
        state.users = res.data
      }).catch(e => {
        toast = { id: user.id, message: e.Message, isPositive: false }
        state.toasts.push(toast)
      })
      let users = state.users

      let idx = _.findIndex(users, { id: user.id })
      let toast
      if (idx !== -1) {
        users.splice(idx, 1, user)
        toast = { id: user.id, message: 'User updated', isPositive: true }
        // { id: uuid(), message: 'User updated', isPositive: true }
      } else {
        users.push(user)
        toast = { id: user.id, message: 'User added', isPositive: true }
      }

      state.users = users
      state.toasts.push(toast)
      setTimeout(() => {
        state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
      }, 3000)

      updateStorage(state.users)
    },
    removeUser (state, user) {
      console.log('Delete storage func')
      let reqUrl = 'contact/' + user.id
      myApi.delete(reqUrl).then(res => {
        state.users = res.data
      }).catch(e => {
        console.log(e)
      })
      state.users = _.remove(state.users, (u) => u.id !== user.id)
      if (state.currentUser.id === user.id) {
        state.currentUser = null
      }

      let toast = { id: uuid(), message: 'User removed', isPositive: false }
      state.toasts.push(toast)
      setTimeout(() => {
        state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
      }, 3000)

      updateStorage(state.users)
    },
    selectUser (state, user) {
      console.log('Select storage func')
      let reqUrl = 'contact/' + user.id
      myApi.get(reqUrl).then(res => {
        state.currentUser = res.data
      }).catch(e => {
        alert(e)
      })
    },
    removeToast (state, toast) {
      state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
    },
    syncUser (state, token) {
      console.log(token)
      myApi.post('sync', token)
        .then(res => {
          alert(res)
        }).catch(e => {
          state.toasts.push('toast')
        })
    }
  }
})
