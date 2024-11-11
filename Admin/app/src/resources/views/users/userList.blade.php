@extends('layouts.app')
@section('title','ユーザー一覧')
@section('h1','ユーザー一覧')
@section('body')
    {{$accounts->links('vendor.pagination.bootstrap-5')}}
    <table class="table table-bordered">
        <tr>
            <th>id</th>
            <th>名前</th>
            <th>レベル</th>
            <th>経験値</th>
            <th>ライフ</th>
        </tr>
        @foreach($accounts as $player)
            <tr>
                <td>{{$player['id']}}</td>
                <td>{{$player['name']}}</td>
                <td>{{$player['level']}}</td>
                <td>{{$player['exp']}}</td>
                <td>{{$player['life']}}</td>
            </tr>
        @endforeach
    </table>
@endsection

