@extends('layouts.app')
@section('title','ステージログ7一覧')
@section('h1','ステージログ一覧')
@section('body')
    <form method="get" action="{{route('stagesstages.log')}}">
        @csrf
        <div class="search">
            <input type="search" id="search-text" name="id" class="searchform"
                   placeholder="idを入力">
            <button id="searchBtn">検索</button>
        </div>
    </form>
    @if(!empty($stage_log))
        <table class="table table-bordered">
            <tr>
                <th>id</th>
                <th>ユーザーID</th>
                <th>ステージID</th>
                <th>クリアor失敗</th>
                <th>最短手数</th>
            </tr>
            @foreach($stage_log as $stage_logs)
                <tr>
                    <td>{{$stage_logs['id']}}</td>
                    <td>{{$stage_logs['stage_id']}}</td>
                    <td>{{$stage_logs['user_id']}}</td>
                    <td>{{$stage_logs['result']}}</td>
                    <td>{{$stage_logs['min_hand_num']}}</td>
                </tr>
            @endforeach
        </table>
    @endif
@endsection



