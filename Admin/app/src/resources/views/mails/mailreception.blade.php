@extends('layouts.app')
@section('title','ユーザー受信メール一覧')
@section('h1','ユーザー受信メール一覧')
@section('body')
    <table class="table table-bordered">
        <form method="get" action="{{route('mailsuser_mail_list')}}">
            @csrf
            <div class="search">
                <input type="search" id="search-text" name="id" class="searchform"
                       placeholder="idを入力">
                <button id="searchBtn">検索</button>
                <input type="hidden" name="action" value="{{$user}}">
            </div>
        </form>

        @if(!empty($user))
            <tr>
                <th>id</th>
                <th>ユーザー名</th>
                <th>メールのid</th>
                <th>受け取り</th>
            </tr>
            @foreach($user->mails as $users)
                <tr>
                    <td>{{$user['id']}}</td>
                    <td>{{$user->name}}</td>
                    <td>{{$users['mail_id']}}</td>
                    <td>{{$users['condition']}}</td>
                </tr>
            @endforeach
        @endif
    </table>
@endsection


