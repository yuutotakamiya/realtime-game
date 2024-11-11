@extends('layouts.app')
@section('title','フォローログ一覧')
@section('h1','フォローログ一覧')
@section('body')
    <form method="get" action="{{route('follow_logslogs')}}">
        @csrf
        <div class="search">
            <input type="search" id="search-text" name="id" class="searchform"
                   placeholder="idを入力">
            <button id="searchBtn">検索</button>
        </div>
    </form>
    @if(!empty($users))
        <table class="table table-bordered">
            <tr>
                <th>id</th>
                <th>ユーザーid</th>
                <th>ターゲットユーザーid</th>
                <th>登録or解除</th>
                <th>登録日時</th>
            </tr>
            @foreach($users as $follow_log)
                <tr>
                    <td>{{$follow_log->id}}</td>
                    <td>{{$follow_log->user_id}}</td>
                    <td>{{$follow_log->target_user_id}}</td>
                    <td>{{$follow_log->action}}</td>
                    <td>{{$follow_log->created_at}}</td>
                </tr>
            @endforeach
        </table>
    @endif
@endsection




