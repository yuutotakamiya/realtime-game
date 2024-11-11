@extends('layouts.app')
@section('title','メールの送信')
@section('h1','メールの送信')

@section('body')
    @if($errors->any())
        <ul>
            @foreach($errors->all() as $error)
                <li>{{$error}}</li>
            @endforeach
        </ul>
    @endif
    <form method="post" action="{{route('mailsmail_send')}}">
        @csrf
        <div class="form-floating">
            <input type="text" class="form-control" id="floatingInput" placeholder="name@example.com" name="user_id">
            <label for="floatingInput">ユーザーのidを入力してください</label><br>
        </div>
        <div class="form-floating">
            <input type="text" class="form-control" id="floatingPassword" placeholder="mail_id"
                   name="mail_id">
            <label for="floatingInput">メールのidを入力してください</label><br>
        </div>
        <button class="btn btn-primary w-100 py-2" type="submit">送信</button>
        <input type="hidden" name="action" value="send">
    </form>
@endsection
