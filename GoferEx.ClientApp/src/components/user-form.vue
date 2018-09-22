<template>
  <div class="user-form-wrapper">
    <img :src="logo" class="logo"/>

    <div class="user-form">

        <div class="header-md">User Details</div>
        <div class="user-details">
            <input type="text" placeholder="First Name" name="firstName" v-model="firstName"
                    v-validate="'required|alpha_spaces'" :title="errors.first('firstName')"
                    :class="{error:errors.has('firstName')}"/>

            <input type="text" placeholder="Last Name" name="lastName" v-model="lastName"
                    v-validate="'required|alpha_spaces'" :title="errors.first('lastName')"
                    :class="{error:errors.has('lastName')}"/>

            <input type="text" placeholder="Birth Date" name="birthdate" v-model="birthdate"
                    v-validate="'required|date_format:DD/MM/YYYY'" :title="errors.first('birthdate')"
                    :class="{error:errors.has('birthdate')}"/>

            <input type="text" placeholder="Phone Number" name="phone" v-model="phone"
                    v-validate="'required|digits:10'" :title="errors.first('phone')"
                    :class="{error:errors.has('phone')}"/>

            <input type="text" placeholder="Email Address" class="email-input" name="email" v-model="email"
                    v-validate="'required|email'" :title="errors.first('email')"
                    :class="{error:errors.has('email')}"/>
        </div>

        <div class="header-md">Account Details</div>
        <div class="account-details">
            <input type="text" placeholder="User Name" name="username" v-model="username"
                    v-validate="'required|alpha_dash'" :title="errors.first('username')"
                    :class="{error:errors.has('username')}"/>

            <input type="password" placeholder="Password" name="password" v-model="password"
                    v-validate="'required'" :title="errors.first('password')"
                    :class="{error:errors.has('password')}"/>

            <input type="password" placeholder="Repeat Password" name="comparePassword" v-model="confirmPassword"
                    v-validate="'required|confirmed:password'" :title="errors.first('comparePassword')"
                    :class="{error:errors.has('comparePassword')}"/>

            <input type="file" id="fileUpload" @change="onSelectFile"/>
            <label for="fileUpload" class="add-photo-wrapper">
                <div class="add-photo-btn">
                    <img :src="addIcon"/>
                    <span>{{this.photo? 'Update' : 'Add'}} Photo</span>
                </div>
                <img class="current-photo" :src="currentPhoto"/>
            </label>
        </div>

        <div class="actions">
            <div class="clear-btn" @click="clear();selectUser()">{{id? 'Cancel' : 'Clear'}}</div>
            <div class="add-btn" @click="add">{{id? 'Update' : 'Add'}}</div>
        </div>

    </div>

  </div>
</template>

<script>
import logo from '@/assets/logo.png'
import addIcon from '@/assets/add.svg'
import addPhoto from '@/assets/add-photo.png'
import {mapMutations, mapState} from 'vuex'

export default {
  props: ['user'],
  data () {
    return {
      logo,
      addIcon,
      id: null,
      firstName: null,
      lastName: null,
      birthdate: null,
      phone: null,
      email: null,
      username: null,
      password: null,
      confirmPassword: null,
      photo: null
    }
  },
  computed: {
    ...mapState(['currentUser']),
    currentPhoto () {
      if (this.photo) return this.photo
      return addPhoto
    }
  },
  watch: {
    currentUser (currentUser) {
      this.clear()

      if (currentUser) {
        this.id = currentUser.id,
        this.firstName = currentUser.firstName
        this.lastName = currentUser.lastName
        this.birthdate = currentUser.birthdate
        this.phone = currentUser.phone
        this.email = currentUser.email
        this.username = currentUser.username
        this.password = currentUser.password
        this.confirmPassword = currentUser.password
        this.photo = currentUser.photo
      }
    }
  },
  methods: {
    ...mapMutations(['addUser', 'selectUser']),
    onSelectFile (e) {
      let reader = new FileReader()
      reader.readAsDataURL(e.srcElement.files[0])
      reader.onload = () => this.photo = reader.result
      reader.onerror = (error) => console.log('Error: ', error)
    },
    async add () {
      let validateRes = await this.$validator.validateAll()
      if (validateRes) {
        let user = {
          id: this.id,
          firstName: this.firstName,
          lastName: this.lastName,
          birthdate: this.birthdate,
          phone: this.phone,
          email: this.email,
          password: this.password,
          username: this.username,
          photo: this.photo
        }

        this.addUser(user)
        if (!this.id) { this.clear() }
      }
    },
    clear () {
      this.id = null
      this.firstName = null
      this.lastName = null
      this.birthdate = null
      this.phone = null
      this.email = null
      this.username = null
      this.password = null
      this.confirmPassword = null
      this.photo = null
      setTimeout(() => {
        this.errors.clear()
      })
    }
  }
}
</script>

<style lang="scss" scoped>

.user-form-wrapper{
    flex:1;
    display: flex;
    flex-direction: column;
    align-items: center;
    background-color: #f5f7fa;

    .logo{
        margin-top: 98px;
        margin-bottom: 74px;
        height:62px;
        width:200px;
    }
}

.user-form{
    width:520px;
    display: flex;
    flex-direction: column;

    .user-details,.account-details{
        margin-top:20px;
    }

    .user-details{
        margin-bottom: 30px;
        display: grid;
        grid-gap: 20px;
        grid-template-columns: 1fr 1fr;

        .email-input{
            grid-column: 1 / 3;
        }

    }

    .account-details{
        display: grid;
        grid-gap: 20px;
        grid-template-columns: 1fr 144px;
        grid-template-rows: repeat(3, 1fr);

        .add-photo-wrapper{
            grid-row: 1 / 4;
            grid-column: 2 / 3;
            border-radius: 5px;
            box-shadow: 0 1px 6px 0 rgba(0, 0, 0, 0.1);
            background-color: #ffffff;
            border: solid 1px #e6e9f2;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: flex-end;

            .add-photo-btn{
                margin-bottom: 12px;
                font-size: 12px;
                color: #9ba0b2;
                display: flex;
                align-items: center;
                padding: 5px;

                span{
                    margin-left: 10px;
                }
            }

            .current-photo{
                width:100px;
                height:92px;
                margin-bottom: -0.2px;
            }

        }
    }

    #fileUpload{
        display:none;
    }

    .actions{
        display: flex;
        margin-top: 41px;
        justify-content: flex-end;

        .clear-btn,.add-btn{
            width: 94px;
            height: 30px;
            border-radius: 4px;
            font-size: 13px;
            line-height: 1;
            color:#fff;
            display:flex;
            align-items: center;
            justify-content: center;
        }

        .clear-btn{
            background-color: #ff5575;
            margin-right: 20px;
        }

        .add-btn{
            background-color: #5891e2;
        }
    }
}
</style>
