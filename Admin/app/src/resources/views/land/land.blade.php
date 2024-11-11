@extends('layouts.app')
@section('title','島のマスタ')
@section('h1','島のマスタ')
@section('body')
    <table class="table table-bordered">
        @if(!empty($land))
            <tr>
                <th>id</th>
                <th>ステージID</th>
                <th>ブロックを埋める目標数</th>
                <th>完了or未完了</th>
            </tr>
            @foreach($land as $lands)
                <tr>
                    <td>{{$lands['id']}}</td>
                    <td>{{$lands['stage_id']}}</td>
                    <td>{{$lands['block_mission_sum']}}</td>
                    <td>{{$lands['result']}}</td>
                </tr>
            @endforeach
        @endif
    </table>
@endsection



